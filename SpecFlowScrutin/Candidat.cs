using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlowScrutin
{
    public class Candidat
    {
        public String nom { get; }
        public int id { get; }

        public double rate { get; set; }
        public int voix { get; set; }
        public Candidat(string newName, int newId)
        {
            nom = newName;
            id = newId;
        }

        public String getName()
        {
            return nom;
        }

    }
}
