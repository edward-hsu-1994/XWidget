# Read Version
version=$(cat version)

# Output Path
path=$(pwd)
path="$path/ngpkgs"

# Remove Old Output
rm -R -f $path

# Restore Projects
find . -type d | grep '^./XWidget.[^/]*$' | { while read -r project; do eval "dotnet restore $project;"; done }

# Unit Test
find . -type d | grep '^./XWidget.[^/]*$' | grep '\b\.Test$' | { while read -r project; do eval "dotnet test $project;"; done }

# Pack
find . -type d | grep '^./XWidget.[^/]*$' | grep -v '\b\.Test$' | { while read -r project; do eval "dotnet build $project; "; done }
# Pack
find . -type d | grep '^./XWidget.[^/]*$' | grep -v '\b\.Test$' | { while read -r project; do eval "dotnet pack $project -p:Version=$version --output $path; "; done }
