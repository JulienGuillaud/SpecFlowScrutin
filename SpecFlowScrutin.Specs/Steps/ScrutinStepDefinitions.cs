using TechTalk.SpecFlow;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;

namespace SpecFlowScrutin.Specs.Steps
{
    [Binding]
    public sealed class ScrutinStepDefinitions
    {

        private Scrutin _scrutin;

        private List<Candidat> _candidats;

        private List<Electeur> _electeurs;

        public ScrutinStepDefinitions(Scrutin scrutin)
        {
            _scrutin = scrutin;
            _candidats = new List<Candidat>();
            _electeurs = new List<Electeur>();
        }

        [Given(@"candidats suivants")]
        public void GivenCandidatsSuivants(Table table)
        {
            foreach (var h in table.Header)
            {
                _candidats.Add(new Candidat(h, _candidats.Count));
            }
        }

        [Given(@"scrutin test")]
        public void GivenScrutin()
        {
            GivenCandidatsSuivants(new Table("null", "paul", "pierre", "fabrice"));
            _scrutin.OuvrirScrutin(_candidats);
        }

        [Given(@"electeur (.*) vote (.*)")]
        public void GivenElecteurVote(string electeur, string leCandidat)
        {
            var electeurRecherche = _electeurs.Find(elem => elem.nom == electeur);
             //Si l'electeur existe deja c'est qu'il a déja voté 
            if (electeurRecherche == null)
            {
                var newInstanceElecteur = new Electeur(electeur, _electeurs.Count + 1);
                _electeurs.Add(newInstanceElecteur);
                if (leCandidat == "null")
                {
                    leCandidat = null;
                }
                _scrutin.Vote(newInstanceElecteur, leCandidat);

            }
        }

        [When(@"scrutin ferme")]
        public void WhenScrutinFerme()
        {
            _scrutin.FermerScrutin();
        }

        [When(@"scrutin ouvert")]
        public void WhenScrutinOuvert()
        {
            _scrutin.OuvrirScrutin(_candidats);
        }

        [Then("(.*) est candidat")]
        public void ThenIsCandidat(string nomCandidat)
        {
            _scrutin.Candidats.Should().ContainSingle(
                elem => elem.nom == nomCandidat,
                nomCandidat + " n'est pas inscrit"
            );
        }

        [Then("vainqueur est (.*)")]
        public void ThenVainqueurEst(string nomVainqueurAttendu)
        {
            if (nomVainqueurAttendu == "null")
            {
                _scrutin.vainqueurScrutin.getName().Should().BeNull();
                return;
            }
            var vainqueur = _scrutin.vainqueurScrutin;

            vainqueur.getName().Should().Be(nomVainqueurAttendu);
        }

        [Then(@"resultat scrutin est (.*)")]
        public void ThenResultatScrutinEstPaulPierre(string resultatAttendu)
        {
            _scrutin.afficherResultats().Should().Be(resultatAttendu);
        }



        [Then("tour est (.*)")]
        public void ThenTourEst(int tourAttendu)
        {
            int leTour = _scrutin.getTour();
            leTour.Should().Be(tourAttendu);
        }

        [Then("exception (.*)")]
        public void ThenExceptionMessage(string messageErreur)
        {
            _scrutin.Invoking(y => y.FermerScrutin())
                .Should().Throw<System.Exception>()
                .WithMessage(messageErreur);
        }

        [Then(@"garder deux candidats")]
        public void ThenGarderDeuxCandidats(Table table)
        {
            var candidatsDuScrutin = _scrutin.getCandidats();

            table.CompareToSet<Candidat>(candidatsDuScrutin);
        }
    }
}
