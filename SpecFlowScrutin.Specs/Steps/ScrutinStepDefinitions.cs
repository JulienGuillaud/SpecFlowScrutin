using TechTalk.SpecFlow;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;

namespace SpecFlowScrutin.Specs.Steps
{
    [Binding]
    public sealed class ScrutinStepDefinitions
    {
        // Test git vstudio cache
        private readonly Scrutin _scrutin;

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
            GivenCandidatsSuivants(new Table("paul", "pierre", "fabrice"));
            _scrutin.OuvrirScrutin(_candidats);
        }

        [Given("electeur (.*) vote (.*)")]
        public void GivenElecteurVote(string electeur, string? leCandidat)
        {
            var newInstanceElecteur = new Electeur(electeur, _electeurs.Count + 1);
            _electeurs.Add(newInstanceElecteur);

            _scrutin.Vote(newInstanceElecteur, leCandidat);
        }

        [Then("electeur (.*) vote (.*) exception (.*)")]
        public void GivenExceptionElecteurVote(string electeur, string? leCandidat, string erreurMessage)
        {
            var newInstanceElecteur = new Electeur(electeur, _electeurs.Count + 1);
            _electeurs.Add(newInstanceElecteur);

            _scrutin.Invoking(y => y.Vote(newInstanceElecteur, leCandidat))
                .Should().Throw<System.Exception>()
                .WithMessage(erreurMessage);
        }

        [When(@"scrutin ferme")]
        public void WhenScrutinFerme()
        {
            _scrutin.FermerScrutin();
        }

        [When("scrutin ouvert")]
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
                _scrutin.vainqueurScrutin.getName().Should().Be("null");
                return;
            }
            var vainqueur = _scrutin.vainqueurScrutin;

            vainqueur.getName().Should().Be(nomVainqueurAttendu);
        }

        [Then(@"resultat scrutin est (.*)")]
        public void ThenResultatScrutinEstPaulPierre(string resultatAttendu)
        {
            _scrutin.AfficherResultats().Should().Be(resultatAttendu);
        }



        [Then("tour est (.*)")]
        public void ThenTourEst(int tourAttendu)
        {
            int leTour = _scrutin.GetTour();
            leTour.Should().Be(tourAttendu);
        }

        [Then("exception (.*)")]
        public void ThenExceptionMessage(string messageErreur)
        {
            _scrutin.Invoking(y => y.FermerScrutin())
                .Should().Throw<System.Exception>()
                .WithMessage(messageErreur);
        }

        [When("scrutin ouvert exception (.*)")]
        public void ThenOuvrirScrutinExceptionMessage(string messageErreur)
        {
            _scrutin.Invoking(y => y.OuvrirScrutin(_candidats))
                .Should().Throw<System.Exception>()
                .WithMessage(messageErreur);
        }

        [Then(@"garder deux candidats")]
        public void ThenGarderDeuxCandidats(Table table)
        {
            var candidatsDuScrutin = _scrutin.GetCandidats();

            table.CompareToSet<Candidat>(candidatsDuScrutin);
        }
    }
}
