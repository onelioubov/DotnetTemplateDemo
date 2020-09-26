#!/bin/bash

echo "Creating new local repository..."
if ! git status > /dev/null 2>&1
then
  git init
else
  echo "Local repository already exists."
fi

echo -e "\nCreating [main] branch..."
if git checkout -b main
then
  echo -e "\nMaking initial commit..."
  git add .
  git commit -m "initial commit"
  git branch -M main
else
  echo "Please see the following log for commit details."
  git log
fi

echo -e "\nSetting up remote repo..."
if gh help > /dev/null 2>&1
then
  tryCreateRemote=$(gh repo create ServiceDemo --public --confirm 2>/dev/null)
  if [ $? -eq 0 ]
  then
    echo "Created new repository $tryCreateRemote and added as remote."
    echo "If you're deploying to cloud resources, make sure to set those resources up before you push [main] to [origin]."
  else
    echo "Remote repository already exists."
  fi
else
  echo "Please install GitHub CLI. For more info, visit https://cli.github.com/."
fi