# Push process is done in following order:
#   1. Exit if there are uncommitted changes
#   2. Push in current working branch
#   3. If succeed, checkout to master branch.
#       a. Otherwise, abort the script
#   4. Merge changes from current working branch to master
#   5. If succeed, push to master branch
#       a. Otherwise, abort the script
#   
#   To execute:
#       Method 1: ./prepare_and_push.sh OR
#       Method 2: 
#           - You need to setup the script in path
#           - cp prepare_and_push.sh prepare_and_push
#           - mv prepare_and_push /bin
#           - add below line to ~/.bashrc
#           - export PATH="$PATH:/bin/prepare_and_push" 
#           Then, execute it: prepare_and_push   
#

# Ensure there are no uncommitted changes before proceeding
if ! git diff-index --quiet HEAD --; then
    echo -e "\e[31mYou have uncommitted changes.\e[0m"
    exit 1
fi

# Push to main branch first
current_branch=$(git rev-parse --abbrev-ref HEAD)
result=$(git push origin $current_branch 2>&1)
status=$?

if [ $status -eq 0 ]; then
    echo "Pushed to '$current_branch' successfully!"
else
    echo -e "\e[31mFailed to push to '$current_branch'\e[0m"
    echo "$result"
    exit 1
fi

# Push to current working branch succeeded,
# tries to merge changes into master branch
result=$(git checkout master 2>&1)
status=$?

if [ $status -ne 0 ]; then
    echo -e "\e[31mFailed to checkout to master\e[0m"
    echo "\t$result"
    exit 1
fi

result=$(git merge $current_branch 2>&1)
status=$?
if [ $status -eq 0 ]; then
    echo "Succesfully merge to master!"
else
    echo -e "\e[31mFailed to merge to master!\e[0m"
    echo "\t$result"
    exit 1
fi

result=$(git push origin master 2>&1)
status=$?

if [ $status -eq 0 ]; then
    echo -e "\e[32mPushed to master successfully!\e[0m"
else
    echo -e "\e[31mFailed to push to master!\e[0m"
    echo "$result"
    exit 1
fi

result=$(git checkout $current_branch > /dev/null 2>&1)
status=$?

if [ $status -eq 0 ]; then
    echo "Switched back to branch '$current_branch'"
else
    echo -e "\e[32mUnable to checkout back to $current_branch\e[0m"
    echo "$result"
fi
