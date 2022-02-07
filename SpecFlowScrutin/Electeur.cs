using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlowScrutin
{
    public class Electeur
    {
        public String nom { get; }
        public int id { get; }
        private bool aVoteBool;
        public Electeur(string newName, int newId)
        {
            nom = newName;
            id = newId;
            aVoteBool = false;
        }

        public void setAVoter()
        {
            aVoteBool = true;
        }

        public bool aVoter()
        {
            return aVoteBool;
        }
    }
}
