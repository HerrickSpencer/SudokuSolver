# README #
[Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

### What is this repository for? ###

* Quick summary  
	Written originally (ca. October 2011) as a conversion of a [Perl program](https://github.com/HerrickSpencer/SudokuSolver/blob/master/SudokuSolver/Sudoku.pl) to C#, this is a Sudoku solver that primarily works using events triggered by changes to the grid. It 'self solves' as any cell is entered or solved for asynchronously. 
* Version 1.0  
* Longer summary  
	This started as a Perl program I wrote ([included in this repo](https://github.com/HerrickSpencer/SudokuSolver/blob/master/SudokuSolver/Sudoku.pl)) that I used to help solve difficult Sudoku puzzles... then I tried to use that pl to solve the most difficult ones and it works well. However, I wanted to study Async programming and Eventing in C#... so I decided to rewrite the project in C# as a library for a console app. (The Console app is still included in the project)
	This is where the project turned into a purely event driven program. Only when a cell is changed, either by the user or the solver itself, will the solver run any logic to solve additional cells. It uses a hierarchy of inherited classes to accomplish this, each doing work for their respective type. It works only by events thrown when any cell is either changed or solved. This in turn 'tells' the related cells in columns, rows, or boxes to attempt to solve themselves.	

	The UI also has some controls to allow for loading puzzles from text files for testing (see the root Boards directory) and for loading the file that contains "the hardest Sudoku puzzle"^1. It also has the ability to save a puzzle to a text tile for use in the program. This is very useful for creating unit tests as well as it is the format the solver library can import. 
	
	The last function of the UI is a TryBreak button that will attempt to forcibly solve the puzzle by brute force. This is done by cycling through each cell attempting a possible value till the correct one is found. 

*(1) = I can not verify the validity of this being the hardest puzzle... I've seen multiple claiming to be the most difficult.*

### How do I get set up? ###

* Download repo and build
	* Run the SudokuUI as your startup project 
	* open an issue if this doesn't work.
* Configuration
	* There are no current config settings, but you can edit the boards to be loaded by adding/editing them in the root level "Boards" directory.
* Dependencies
	* .Net 4.5 
* How to run tests
	* Tests are in separate project UnitTestsProject1 (creative name) and are run in VS normally with Visual Studio's Test Explorer

### Contribution guidelines ###
* I'd love to see your contributions to the code!
* Writing tests is recommended
	* Write unit tests to support any new functionality to ensure your work is not regressively broken by future commits.
	* Don't write unnecessary tests. IE: tests that are covered by other tests already.
* Code review
	* Use normal BitBucket/GitHub Pull Request system
* Other guidelines
 	* All tests should pass before merges are done into master branch


### Who do I talk to? ###

* Repo owner or admin
	* Currently me, Herrick Spencer - email: (Code [at sym] Herrickspencer [dot] com)
* Other community or team contact
	* Currently there is none, but we can develop if the project takes off.
