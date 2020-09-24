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
  # The fact that the name of the repo that this is creating is the same as the Demo repo name can potentially be 
  # problematic if the variable replacement doesn't work for whatever reason. I don't think it's going to override it,
  # but it's still bad design. Will need to do one of the following things (this is mostly so I remember, in case I
  # forgot about this):
  #
  # a) Rename DotnetTemplateDemo repo to something different from the service name variables it's replacing.
  # b) Rename all the occurrences of DotnetTemplateDemo inside the repo (but not the repo itself).
  # c) Create a one-off auto-implemented template parameter to replace a different variable, which will be used here.
  # 
  # Until one of the above is implemented, DO NOT run this script (even though it totally works).
  tryCreateRemote=$(gh repo create DotnetTemplateDemo --public --confirm 2>/dev/null)
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