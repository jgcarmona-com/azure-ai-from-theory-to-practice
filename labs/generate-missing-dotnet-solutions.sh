#!/bin/bash

# Function to create a solution file in each directory with a .csproj file
generate_solutions() {
  find . -type f -name "*.csproj" | while read -r csproj; do
    project_dir=$(dirname "$csproj")
    parent_dir=$(basename "$(dirname "$project_dir")")

    # Check if the parent directory is named 'csharp'
    if [ "$parent_dir" == "csharp" ]; then
      solution_name=$(basename "$project_dir")
      solution_path="$project_dir/$solution_name.sln"

      # Check if a solution file already exists
      if [ ! -f "$solution_path" ]; then
        dotnet new sln -n "$solution_name" -o "$project_dir" -f "net8.0"
        dotnet sln "$solution_path" add "$csproj"
        echo "Created solution: $solution_path"
      else
        echo "Solution already exists: $solution_path"
      fi
    fi
  done
}

# Run the function
generate_solutions
