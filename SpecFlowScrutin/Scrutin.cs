using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlowScrutin
{
    [System.Serializable]
    public class ScrutinException : System.Exception
    {
        public ScrutinException(string message) : base(message) { }
    }
    public class Scrutin
    {
        private bool estOuvert;
        private int tour = 0;
        private int nbVotes = 0;

        public List<Candidat> Candidats = new List<Candidat>();
        public List<Electeur> Electeurs = new List<Electeur>();

        public Dictionary<Candidat, int> resultatsVoix = new Dictionary<Candidat, int>();
        public Dictionary<Candidat, double> resultatsRate = new Dictionary<Candidat, double>();

        protected static Candidat voteBlanc = new Candidat("null", 9999999);

        public Candidat vainqueurScrutin = voteBlanc;
        public Scrutin()
        {
        }

        public void FermerScrutin()
        {
            estOuvert = false;
            CalculPourcentageVoies();
            OuvrirScrutin(CalcVainqueur());
        }

        public void OuvrirScrutin(List<Candidat> lesCandidats)
        {
            if (lesCandidats.Count <= 2 && tour == 0)
            {
                throw new ScrutinException("Il n'y a pas assez de candidats");
            }
            else
            {
                if (lesCandidats.Count <= 1 && tour != 0)
                {
                    // End
                }
                else
                {
                    ResetScrutin();
                    estOuvert = true;
                    tour += 1;
                    Candidats = lesCandidats;
                    Candidats.Add(voteBlanc);

                }
            }
        }

        public String AfficherResultats()
        {
            String resultatString = "";
            foreach (KeyValuePair<Candidat, int> kvp in resultatsVoix)
            {
                Candidat leCandidat = kvp.Key;
                int voixCandidat = kvp.Value;
                double pourcentageVoix = Math.Round(((double)voixCandidat / (double)nbVotes) * 100, 2, MidpointRounding.ToEven);
                resultatString += "| " + leCandidat.nom + " : " + voixCandidat + " (" + pourcentageVoix + "%)";
            } // | paul : 3 (60%)| pierre : 2 (40%) |
            return resultatString;
        }

        public void ResetScrutin()
        {
            vainqueurScrutin = voteBlanc;
            nbVotes = 0;
            resultatsVoix = new Dictionary<Candidat, int>();
            resultatsRate = new Dictionary<Candidat, double>();
            Electeurs = new List<Electeur>();
        }

        public void AddCandidat(Candidat leCandidat)
        {
            if (estOuvert)
            {
                Candidats.Add(leCandidat);
            }
            else
            {
                throw new ScrutinException("Le scrutin est fermé");
            }
        }

        public List<Candidat> GetCandidats()
        {
            List<Candidat> candidatsClone = Candidats;
            candidatsClone.RemoveAt(candidatsClone.Count() - 1);
            return candidatsClone;
        }

        public void Vote(Electeur leElecteur, String nomCandidat)
        {
            if (estOuvert)
            {
                Candidat candidatEnregistre = Candidats.Find(x => x.nom == nomCandidat);
                Electeur electeurEnregistre = Electeurs.Find(x => x.nom == leElecteur.nom);
                Console.Write("CANDIDAT ENREGISTRE = " + candidatEnregistre.nom);
                if (nomCandidat == null || nomCandidat == "null")
                {
                    candidatEnregistre = voteBlanc;
                }
                if (electeurEnregistre == null && !(leElecteur.aVoter()))
                {
                    leElecteur.setAVoter();
                    Electeurs.Add(leElecteur);
                    nbVotes += 1;
                    int currentCount;
                    if (resultatsVoix.TryGetValue(candidatEnregistre, out currentCount))
                    {
                        resultatsVoix[candidatEnregistre] = currentCount + 1;
                    }
                    else
                    {
                        resultatsVoix[candidatEnregistre] = 1;
                    }

                }
                else
                {
                    throw new ScrutinException("Un electeur peux pas voter deux fois");
                }
            }
            else
            {
                throw new ScrutinException("Le scrutin est fermé");
            }
        }

        public void CalculPourcentageVoies()
        {
            if (!estOuvert)
            {
                foreach (KeyValuePair<Candidat, int> kvp in resultatsVoix)
                {
                    Candidat leCandidat = kvp.Key;
                    int voixCandidat = kvp.Value;
                    double pourcentageVoix = Math.Round(((double)voixCandidat / (double)nbVotes) * 100, 2, MidpointRounding.ToEven);
                    resultatsRate.Add(leCandidat, pourcentageVoix);
                    leCandidat.rate = pourcentageVoix;
                    leCandidat.voix = voixCandidat;
                }
            }
            else
            {
                throw new ScrutinException("Le scrutin est Ouvert");
            }
        }


        public int GetTour()
        {
            return tour;
        }
        public List<Candidat> CalcVainqueur()
        {
            if (!estOuvert)
            {
                var finalistes = new List<Candidat>();
                double plusHautPourcentage = 0;
                Candidat vainqueur = Candidats.Find(elem => elem.nom == "null");

                foreach (KeyValuePair<Candidat, double> kvp in resultatsRate)
                {
                    Candidat leCandidat = kvp.Key;
                    double voixCandidat = kvp.Value;

                    if (voixCandidat > plusHautPourcentage)
                    {
                        plusHautPourcentage = voixCandidat;
                        vainqueur = leCandidat;
                    }
                }

                if (plusHautPourcentage > 50)
                {
                    vainqueurScrutin = vainqueur;
                }
                else
                {
                    // Lancer le prochain tour avec les 2 finalistes
                    if (tour == 1)
                    {
                        var sorted = filterDictionary(resultatsRate);
                        int compteur = 0;
                        foreach (KeyValuePair<Candidat, double> kvp in sorted)
                        {
                            if (compteur < 2)
                            {
                                finalistes.Add(kvp.Key);
                            }
                            else
                            {
                                break;
                            }
                            compteur++;
                        }
                    }
                    // Si on est déja au 2eme tour mais qu'il y a pas de gagnant, arreter le scrutin
                    if (tour == 2)
                    {
                        // Aucun gagnant
                        vainqueurScrutin = voteBlanc;
                    }
                }
                return finalistes;

            }
            else
            {
                throw new ScrutinException("Le scrutin est ouvert");
            }
        }

        protected Dictionary<Candidat, double> filterDictionary(Dictionary<Candidat, double> elem)
        {
            return (from entry in elem orderby entry.Key.rate descending select entry).ToDictionary(x => x.Key, x => x.Value);
        }

        public double GetVoixPourcentage(Candidat leCandidat)
        {
            return resultatsRate[leCandidat];
        }
        public int GetVoixNb(Candidat leCandidat)
        {
            return resultatsVoix[leCandidat];
        }


        public Candidat FindCandidat(String nom)
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