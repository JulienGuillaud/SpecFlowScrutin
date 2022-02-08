Feature: Scrutin

	@scrutin
	Scenario: add candidat
		Given candidats suivants
			| paul | pierre | fabrice |
		When scrutin ouvert
		Then paul est candidat

	Scenario: minimum 2 candidat
		Given candidats suivants
			| paul |
		When scrutin ouvert exception Il n'y a pas assez de candidats
		
	Scenario: vote null
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote pierre
		And electeur blop vote pierre
		And electeur brice vote pierre
		And electeur theo vote pierre
		And electeur dorian vote null
		When scrutin ferme
		Then vainqueur est pierre

	Scenario: vote double
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote pierre
		Then electeur arthur vote fabrice exception Un electeur peux pas voter deux fois
		
	Scenario: afficher scrutin
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote pierre
		And electeur brice vote pierre
		And electeur dorian vote paul
		When scrutin ferme
		Then resultat scrutin est | paul : 3 (60%)| pierre : 2 (40%)


	Scenario: vainqueur majorite absolue
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur brice vote pierre
		When scrutin ferme
		Then vainqueur est paul

	Scenario: match null premier tour
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote pierre
		And electeur brice vote pierre
		And electeur dorian vote fabrice
		When scrutin ferme
		Then vainqueur est null
		And garder deux candidats
			| id | nom    |
			| 0  | paul   |
			| 1  | pierre |
		Then tour est 2
		Given electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote paul
		And electeur brice vote pierre
		And electeur dorian vote pierre
		When scrutin ferme
		Then vainqueur est paul

	Scenario: match nul second tour
		Given scrutin test
		And electeur arthur vote paul
		And electeur julien vote paul
		And electeur pascal vote pierre
		And electeur brice vote pierre
		And electeur dorian vote fabrice
		When scrutin ferme
		Then vainqueur est null
		And garder deux candidats
			| id  | nom    |
			| 0   | paul   |
			| 1   | pierre |
		Then tour est 2
		Given electeur arthur vote paul
		And electeur julien vote paul
		And electeur brice vote pierre
		And electeur dorian vote pierre
		When scrutin ferme
		Then vainqueur est null

