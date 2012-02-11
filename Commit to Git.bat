echo off
cd C:\Users\Hoagies\Documents\Visual Studio 2010\Projects\LearningApp
git add *
git status
set /P commitText=Please type in a comment about this commit (leave blank to abort): 
git commit -m %commitText%
git push
pause Press enter to continue...