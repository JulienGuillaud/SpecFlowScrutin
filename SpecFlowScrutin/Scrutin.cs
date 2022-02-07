using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlowScrutin
{
    [System.Serializable]
    public class scrutinException : System.Exception
    {
        public scrutinException() { }
        public scrutinException(string message) : base(message) { }
    }
    public class Scrutin
    {
        private bool estOuvert;
        private int tour = 0;
        public List<Candidat> Candidats = new List<Candidat>();
        public List<Electeur> Electeurs = new List<Electeur>();

        public Dictionary<Candidat, int> resultatsVoix = new Dictionary<Candidat, int>();
        public Dictionary<Candidat, int> resultatsRate = new Dictionary<Candidat, int>();

        protected static Candidat voteBlanc = new Candidat("", 9999999);

        public Candidat vainqueurScrutin = voteBlanc;
        public Scrutin()
        {
        }

        public void FermerScrutin()
        {
            estOuvert = false;
            calculPourcentageVoies();
            OuvrirScrutin(calcVainqueur());
        }

        public void OuvrirScrutin(List<Candidat> lesCandidats)
        {
            if(lesCandidats.Count <= 0)
            {
                throw new scrutinException("Aucun candidats");
            }
            else
            {
                estOuvert = true;
                tour += 1;
                Candidats = lesCandidats;
            }
        }

        public void addCandidat(Candidat leCandidat)
        {
            if (estOuvert)
            {
                Candidats.Add(leCandidat);
            }
            else
            {
                throw new scrutinException("Le scrutin est fermé");
            }
        }

        public List<Candidat> getCandidats()
        {
            return Candidats;
        }

        public void Vote(Electeur _electeur, String nomCandidat)
        {
            if (estOuvert)
            {
                Candidat candidatEnregistre = findCandidat(nomCandidat);
                if(nomCandidat == null || nomCandidat == "")
                {
                    candidatEnregistre = voteBlanc;
                }
                if (candidatEnregistre != null && !(_electeur.aVoter()))
                {
                    _electeur.setAVoter();
                    Electeurs.Add(_electeur);
                    resultatsVoix[candidatEnregistre] = resultatsVoix[candidatEnregistre] + 1;
                }
            }
            else
            {
                throw new scrutinException("Le scrutin est fermé");
            }
        }

        public void calculPourcentageVoies()
        {
            if (!estOuvert)
            {
                foreach (KeyValuePair<Candidat, int> kvp in resultatsVoix)
                {
                    Candidat leCandidat = kvp.Key;
                    int totalCandidats = Candidats.Count;
                    int voixCandidat = kvp.Value;
                    int pourcentageVoix = (int)Math.Floor((double)(voixCandidat / totalCandidats) * 100);
                    resultatsRate[leCandidat] = pourcentageVoix;
                }
            }
            else
            {
                throw new scrutinException("Le scrutin est Ouvert");
            }
        }


        public int getTour()
        {
            return tour;
        }
        public List<Candidat> calcVainqueur()
        {
            if (!estOuvert)
            {
                var finalistes = new List<Candidat>();
                int plusHautPourcentage = 0;
                Candidat vainqueur = Candidats.Find(elem => elem.nom == "null");

                foreach (KeyValuePair<Candidat, int> kvp in resultatsRate)
                {
                    Candidat leCandidat = kvp.Key;
                    int voixCandidat = kvp.Value;

                    if (voixCandidat > plusHautPourcentage)
                    {
                        plusHautPourcentage = voixCandidat;
                        vainqueur = leCandidat;
                    }
                }

                if (plusHautPourcentage >= 50)
                {
                    vainqueurScrutin = vainqueur;
                }
                else
                {
                    // Prochain tour si on en est au 1er
                    if(tour == 1)
                    {
                        var sorted = resultatsRate.OrderByDescending(x => x.Value).ThenBy(x => x.Key);
                        foreach (KeyValuePair<Candidat, int> kvp in sorted)
                        {
                            finalistes.Add(kvp.Key);
                        }
                    }
                    if(tour == 2)
                    {
                        // Aucun gagnant
                        vainqueurScrutin = voteBlanc;
                        throw new scrutinException("Pas de vainqueurs");

                    }
                }
                return finalistes;

            }
            else
            {
                throw new scrutinException("Le scrutin est ouvert");
            }
        }

        public int getVoixPourcentage(Candidat leCandidat)
        {
            return resultatsRate[leCandidat];
        }
        public int getVoixNb(Candidat leCandidat)
        {
            return resultatsVoix[leCandidat];
        }


        public Candidat findCandidat(String nom)
        {
            for (int i = 0; i < Candidats.Count; i++)
            {
                if (Candidats[i].getName() == nom)
                {
                    return Candidats[i];
                }
            }
            return null;
        }

    }
}