using TechTalk.SpecFlow;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;

namespace SpecFlowScrutin.Specs.Steps
{
    [Binding]
    public sealed class ScrutinStepDefinitions
    {

        private readonly Scrutin _scrutin;

        private List<Candidat> _candidats;

        private List<Electeur> _electeurs;

        private readonly ScenarioContext _scenarioContext;

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

        [Given(@"electeur")]
        public void GivenElecteur(string electeur, string candidatName)
        {
            _electeurs.Add(new Electeur(electeur, _electeurs.Count));
            _scrutin.Vote(new Electeur(electeur, _electeurs.Count), candidatName);
        }


        [Given(@"scrutin test")]
        public void GivenScrutin()
        {
            GivenCandidatsSuivants(new Table("null", "paul", "pierre", "fabrice"));
            _scrutin.OuvrirScrutin(_candidats);

            //GivenElecteur("julien", "paul");
            //GivenElecteur("arthur", "pierre");
            //GivenElecteur("brice", "jean");
            //GivenElecteur("dorian", "fabrice");
            //GivenElecteur("pascal", "paul");
        }

        [Given("electeur (.*) vote (.*)")]
        public void WhenElecteurVote(string electeur, string candidat)
        {
            // Si l'electeur existe deja c'est qu'il a déja voté 
            if(_electeurs.Find(elem => elem.nom == electeur) == null)
            {
                var instanceElecteur = new Electeur("slecteur", _electeurs.Count + 1);
                _electeurs.Add(instanceElecteur);
                if (candidat == "null")
                {
                    candidat = null;
                }

                _scrutin.Vote(instanceElecteur, candidat);

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
                _scrutin.getVainqueur().getName().Should().BeNull();
                return;
            }

            var vainqueur = _scrutin.getVainqueur();

            vainqueur.getName().Should().Be(nomVainqueurAttendu);
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
