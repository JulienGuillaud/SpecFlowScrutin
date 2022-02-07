Feature: Scrutin

	@scrutin
	Scenario: add candidat
		Given candidats suivants
			| paul | pierre | fabrice |
		When scrutin ouvert
		Then paul est candidat

	Scenario: minimum candidat
		Given candidats suivants
			| paul |
		And electeur arthur vote paul
		When scrutin ouvert
		Then exception Il n'y a pas assez de candidats

	Scenario: vainqueur majorite absolue
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur brice vote pierre
		When scrutin ferme
		Then vainqueur est paul

	Scenario: match nul premier tour
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote pierre
		And electeur brice vote pierre
		And electeur dorian vote fabrice
		When scrutin ferme
		Then vainqueur est null
		Then tour est 1
		And garder deux candidats
			| Id  | Name   |
			| 1   | paul   |
			| 2   | pierre |
		Given electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote paul
		And electeur brice vote pierre
		And electeur dorian vote pierre
		When scrutin ferme
		Then vainqueur est paul

