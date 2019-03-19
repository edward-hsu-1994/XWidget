# Restore Projects
find . -type d | grep '^./XWidget.[^/]*$' | { while read -r project; do eval "dotnet restore $project;"; done }

# Unit Test
find . -type d | grep '^./XWidget.[^/]*$' | grep '\b\.Test$' | { while read -r project; do eval "dotnet test $project;"; done }

# Pack
find . -type d | grep '^./XWidget.[^/]*$' | grep -v '\b\.Test$' | { while read -r project; do eval "dotnet pack $project;"; done }
