using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
namespace ReceiptParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string jsonData = File.ReadAllText("response.json");
            List<Root> roots = JsonConvert.DeserializeObject<List<Root>>(jsonData);
            roots = roots.Where(t => t.Locale == null).ToList();
            roots.Sort();
            
            List<List<Root>> rows=new();
            List<Root> list = new();
            Root? previous = null;
            foreach(Root item in roots)
            {
                if(previous!=null)
                {
                   if(item.BoundingPoly.IsNewLine(previous.BoundingPoly))
                   {
                        rows.Add(list);
                        list = new List<Root>();
                   }
                }
                list.Add(item);
                previous = item;
            }
            rows.Add(list);

            foreach(List<Root> rootList in rows)
            {
                foreach(Root item in rootList)
                {
                    Console.Write(item.Description + " ");
                }
                Console.WriteLine();
            }

        }
    }

  
    public class BoundingPoly
    {
        private readonly double? newLineThreshold = 10;
        public List<Vertex>? Vertices { get; set; }
        public Vertex FindCentroid()
        {
            int? x = 0;
            int? y = 0;
            foreach (Vertex p in Vertices)
            {
                x += p.X;
                y += p.Y;
            }
            Vertex center = new()
            {
                X = x / Vertices.Count,
                Y = y / Vertices.Count
            };
            return center;
        }

        public double? AvgOfX()
        {
            return Vertices == null ? 0 : Vertices.Select(t => t.X).Average();
        }

        public bool IsNewLine(BoundingPoly boundingPoly)
        {
            return this.FindCentroid().Y > boundingPoly.FindCentroid().Y + newLineThreshold;
        }
    }

    public class Root : IComparable
    {
        public string? Locale { get; set; }
        public string? Description { get; set; }
        public BoundingPoly? BoundingPoly { get; set; }

        public int CompareTo(object? obj)
        {
            Root r1 = (Root)this;
            Root r2 = (Root)obj;
            if (r1.BoundingPoly.FindCentroid().Y > r2.BoundingPoly.FindCentroid().Y)
                return 1;
            else if (r1.BoundingPoly.FindCentroid().Y < r2.BoundingPoly.FindCentroid().Y)
                return -1;
            else if (r1.BoundingPoly.FindCentroid().X < r2.BoundingPoly.FindCentroid().X) //probably same line
                return -1;
            else return 1;
        }

    }

    public class Vertex
    {
        public int? X { get; set; }
        public int? Y { get; set; }
    }






}