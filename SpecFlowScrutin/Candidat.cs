using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlowScrutin
{
    public class Candidat
    {
        public string nom { get; }
        public int id { get; }
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
