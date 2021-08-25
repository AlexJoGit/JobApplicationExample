using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using OpenTK;

namespace MapEditor
{
    class Reader
    {
        public class RSymbol
        {
            public enum RS_Types
            {
                RS_Invalid = 0,

                RS_Ident = 1,
                RS_Int,
                RS_Float,

                RS_Slash,

                RS_mtllib = 100,
                RS_ObjectName,
                RS_VertexIdent,
                RS_TextureIdent,
                RS_NormalIdent,
                RS_usemtl,
                RS_FaceIdent,
                RS_S,
                RS_NO,
                RS_OFF

            }

            public static Dictionary<string, RS_Types> Types = new Dictionary<string, RS_Types>()
            {
                {"mtllib", RS_Types.RS_mtllib},
                {"o", RS_Types.RS_ObjectName},
                {"v", RS_Types.RS_VertexIdent},
                {"vt", RS_Types.RS_TextureIdent},
                {"vn", RS_Types.RS_NormalIdent},
                {"usemtl", RS_Types.RS_usemtl},
                {"None", RS_Types.RS_NO},
                {"s", RS_Types.RS_S},
                {"off", RS_Types.RS_OFF},
                {"f", RS_Types.RS_FaceIdent},
            };

            public RS_Types type;
            public string sValue;
            public float fValue;
            public int iValue;

            public override string ToString()
            {
                if(type == RS_Types.RS_Int)
                {
                    return "iValue: " + iValue;
                }
                else if (type == RS_Types.RS_Float)
                {
                    return "fValue: " + fValue;
                }
                else
                {
                    return "type: " + type + " sValue: " + sValue;
                }
                
            }

            public void reset()
            {
                type = RS_Types.RS_Invalid;
                sValue = "";
                fValue = 0.0f;
                iValue = 0;
            }

        }

        public class RObject
        {
            public enum RO_Types
            {
                RO_Invalid = 0,

                RO_Verts = 1,
                RO_Norm,
                RO_Tex,
                RO_Ind,
            }

            public RO_Types type;
            public string uuuhm;
            public Vector3 v3;

            public List<List<int>> face = new List<List<int>>();

            public override string ToString()
            {
                return "type: " + type + " v3: " + v3;
            }

            public void reset()
            {
                uuuhm = "";
                type = new int();
                v3 = Vector3.Zero;
                face.Clear();
            }
        }

        private StreamReader sr;

        public bool nextChar(ref char c)
        {
            if (!sr.EndOfStream)
            {
                c = (char)sr.Read();

                return true;
            }
            else if(sr.EndOfStream)
            {
                return false;
            }
            else
            {
                throw new Exception("smth went horribly wrong");
            }
        }

        private RSymbol CurrentSymbol = new RSymbol();

        bool IsSymbol(RSymbol.RS_Types type)
        {
            return (type == CurrentSymbol.type);
        }

        void matchSymbol(RSymbol.RS_Types type)
        {
            if (IsSymbol(type))
            {
                nextSymbol(ref CurrentSymbol);
            }
            else
            {
                throw new Exception("uao<iU)hdwE)U");
            }
        }

        void matchSymbol(RSymbol.RS_Types type, out float f)
        {
            if (IsSymbol(type))
            {
                f = CurrentSymbol.fValue;
                nextSymbol(ref CurrentSymbol);
            }
            else
            {
                throw new Exception("uao<iU)hdwE)U");
            }
        }

        public bool nextSymbol(ref RSymbol rs)
        {
            rs.reset();
            char x = new char();

            while (nextChar(ref x))
            {
                if (x == '#')
                {//# ist zeilenkomentar
                    while (nextChar(ref x))
                    {
                        if (x == 10)
                        {
                            break;
                        }
                    }
                }
                else if (x == ' ' || x== 13 || x == 10)
                {// ' ' ist space
                    while (sr.Peek() == 32 || sr.Peek() == 13 || sr.Peek() == 10)
                    {
                        nextChar(ref x);
                    }
                }
                else if (('a' <= x && x <= 'z') || ('A' <= x && x <= 'Z'))
                {// buschtabe is ZeilenIdentifikator, type 1

                    rs.sValue += x;

                    while (('a' <= sr.Peek() && sr.Peek() <= 'z') || ('A' <= sr.Peek() && sr.Peek() <= 'Z') || (sr.Peek() == '_') || (sr.Peek() == '.') || ('0' <= sr.Peek() && sr.Peek() <= '9'))
                    {
                        nextChar(ref x);
                        rs.sValue += x;
                    }

                    if(!RSymbol.Types.TryGetValue(rs.sValue, out rs.type))
                    {
                        rs.type = RSymbol.RS_Types.RS_Ident;
                    }

                    return true;

                }
                else if (('0' <= x && x <= '9') || ('-' == x ))
                {//zahl oder so type 2 ist zahl(int), type 3 ist zahl(float)
                    rs.type = RSymbol.RS_Types.RS_Int; 

                    bool sigh = ('-' == x);
                    if (!sigh)
                    {
                        rs.fValue += x - '0';
                    }
                    rs.sValue += x;
                    while (('0' <= sr.Peek() && sr.Peek() <= '9'))
                    {
                        nextChar(ref x);

                        rs.fValue *= 10.0f;
                        rs.fValue += x - '0';

                        rs.sValue += x;
                    }
                    if('.' == sr.Peek())
                    {
                        rs.type = RSymbol.RS_Types.RS_Float;
                        float frac = 0.1f;


                        nextChar(ref x);
                        rs.sValue += x;
                        while (('0' <= sr.Peek() && sr.Peek() <= '9'))
                        {
                            nextChar(ref x);

                            rs.fValue += ((float)(x - '0')) * frac;
                            frac /= 10.0f;
                            

                            rs.sValue += x;
                        }
                    }
                    if (sigh)
                    {
                        rs.fValue *= -1;
                    }
                    
                    return true;
                }
                else if (x == '/')
                {
                    rs.type = RSymbol.RS_Types.RS_Slash;
                    rs.sValue += x;
                    return true;
                }
                else
                {
                    throw new Exception("syntax error");

                }
                

            }
            return false;
        }



        public bool nextObject(RObject ro)
        {
            ro.reset();

            if (IsSymbol(RSymbol.RS_Types.RS_mtllib))
            {
                matchSymbol(RSymbol.RS_Types.RS_mtllib);

                if (IsSymbol(RSymbol.RS_Types.RS_Ident))
                {
                    ro.uuuhm = CurrentSymbol.sValue;
                    matchSymbol(RSymbol.RS_Types.RS_Ident);
                }
                
                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_ObjectName))
            {
                matchSymbol(RSymbol.RS_Types.RS_ObjectName);

                if (IsSymbol(RSymbol.RS_Types.RS_Ident))
                {
                    ro.uuuhm = CurrentSymbol.sValue;
                    matchSymbol(RSymbol.RS_Types.RS_Ident);
                }
                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_VertexIdent))
            {
                ro.type = RObject.RO_Types.RO_Verts;

                matchSymbol(RSymbol.RS_Types.RS_VertexIdent);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.X);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.Y);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.Z);

                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_TextureIdent))
            {
                ro.type = RObject.RO_Types.RO_Tex;

                matchSymbol(RSymbol.RS_Types.RS_TextureIdent);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.X);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.Y);

                return true;

            }
            else if (IsSymbol(RSymbol.RS_Types.RS_NormalIdent))
            {
                ro.type = RObject.RO_Types.RO_Norm;

                matchSymbol(RSymbol.RS_Types.RS_NormalIdent);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.X);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.Y);

                matchSymbol(RSymbol.RS_Types.RS_Float, out ro.v3.Z);

                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_usemtl))
            {
                matchSymbol(RSymbol.RS_Types.RS_usemtl);

                if (IsSymbol(RSymbol.RS_Types.RS_NO))
                {
                    ro.uuuhm = CurrentSymbol.sValue;
                    matchSymbol(RSymbol.RS_Types.RS_NO);
                }
                else
                {
                    matchSymbol(CurrentSymbol.type);
                }

                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_S))
            {
                matchSymbol(RSymbol.RS_Types.RS_S);

                if (IsSymbol(RSymbol.RS_Types.RS_OFF))
                {
                    ro.uuuhm = CurrentSymbol.sValue;
                    matchSymbol(RSymbol.RS_Types.RS_OFF);
                }
                else
                {
                    matchSymbol(CurrentSymbol.type);
                }
                return true;
            }
            else if (IsSymbol(RSymbol.RS_Types.RS_FaceIdent))
            {

                //Console.WriteLine(ro.type + " " + ro.uuuhm);


                ro.type = RObject.RO_Types.RO_Ind;

                matchSymbol(RSymbol.RS_Types.RS_FaceIdent);

                while (IsSymbol(RSymbol.RS_Types.RS_Int))
                {
                    ro.face.Add(new List<int>());

                    ro.face[ro.face.Count - 1].Add((int)CurrentSymbol.fValue);

                    matchSymbol(RSymbol.RS_Types.RS_Int);

                    while (IsSymbol(RSymbol.RS_Types.RS_Slash))
                    {
                        matchSymbol(RSymbol.RS_Types.RS_Slash);

                        if (IsSymbol(RSymbol.RS_Types.RS_Int))
                        {
                            ro.face[ro.face.Count - 1].Add((int)CurrentSymbol.fValue);

                            matchSymbol(RSymbol.RS_Types.RS_Int);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
                throw new Exception("fuck");
            }


        }
        public void ScanAllObjects()
        {

            List<List<List<int>>> faces = new List<List<List<int>>>();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector3> texCoords = new List<Vector3>();


            RObject ro = new RObject();
            matchSymbol(RSymbol.RS_Types.RS_Invalid);

            while(nextObject(ro))
            {
               

                if(ro.type == RObject.RO_Types.RO_Verts)
                {
                    verts.Add(ro.v3);
                }
                else if(ro.type == RObject.RO_Types.RO_Norm)
                {
                    norms.Add(ro.v3);
                }
                else if (ro.type == RObject.RO_Types.RO_Tex)
                {
                    texCoords.Add(ro.v3);
                }
                else if (ro.type == RObject.RO_Types.RO_Ind)
                {
                    faces.Add(new List<List<int>>());

                    foreach (var triple in ro.face)
                    {
                        faces[faces.Count - 1].Add(new List<int>());

                        foreach (var integ in triple)
                        {
                            faces[faces.Count - 1][faces[faces.Count - 1].Count - 1].Add(integ);
                        }
                    }
                }
            }

            Vertices = new float[texCoords.Count * 8];
            Indices = new uint[faces.Count * 3];

            int counter = 0;

            foreach (List<List<int>> face in faces)
            {
                foreach (List<int> triple in face)
                {
                    Vertices[((triple[1]-1) * 8) + 0] = verts[triple[0]-1].X/2;
                    Vertices[((triple[1]-1) * 8) + 1] = verts[triple[0]-1].Y/2;
                    Vertices[((triple[1]-1) * 8) + 2] = verts[triple[0]-1].Z/2;
                    Vertices[((triple[1]-1) * 8) + 3] = norms[triple[2]-1].X;
                    Vertices[((triple[1]-1) * 8) + 4] = norms[triple[2]-1].Y;
                    Vertices[((triple[1]-1) * 8) + 5] = norms[triple[2]-1].Z;
                    Vertices[((triple[1]-1) * 8) + 6] = texCoords[triple[1]-1].X;
                    Vertices[((triple[1]-1) * 8) + 7] = texCoords[triple[1]-1].Y;

                    Indices[counter] = (uint)triple[1] - 1;
                    counter++;
                }

                
            }


            //for (int i = 1; i < Vertices.Length; i++)
            //{
            //    Console.Write(Vertices[i] + " ");
            //    if (i % 8 == 0)
            //    {
            //        Console.WriteLine();
            //    }
            //}

            //for (int i = 0; i < Indices.Length; i++)
            //{
            //    Console.Write(Indices[i] + " ");
            //    if (i % 3 == 0)
            //    {
            //        Console.WriteLine();
            //    }
            //}

        }
        public float[] Vertices;
        public uint[] Indices; 



        public void Open()
        {
            throw new Exception("no path");
        }

        public void Open(string path)
        {
            sr = File.OpenText(path);
        }

        public void Close()
        {
            sr.Close();
        }

        public void GetData(string path)
        {
            Open(path);
            ScanAllObjects();
            Close();

        }



    }
}
