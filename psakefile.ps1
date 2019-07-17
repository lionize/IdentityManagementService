Task Publish -Depends Pack {
   #
}

Task Pack -Depends Build {
   $src = (Resolve-Path ".\src\").Path
   Exec { docker build -f Dockerfile $src -t $script:latestImageTag }
}

Task Build -Depends Init,Clean {
   $script:publishFolder = Join-Path -Path $script:trashFolder -ChildPath "publish"

   New-Item -Path $script:publishFolder -ItemType Directory
   $project = Resolve-Path ".\src\IdentityManagementService.csproj"
   $project = $project.Path
   Exec { dotnet publish $project --output $script:publishFolder }
}

Task Clean -Depends Init {
}

Task Init {
   $date = Get-Date
   $ticks = $date.Ticks
   $script:latestImageTag = "lionize/identity-management-service:latest"
   $trashFolder = Join-Path -Path . -ChildPath ".trash"
   $script:trashFolder = Join-Path -Path $trashFolder -ChildPath $ticks.ToString("D19")
   New-Item -Path $script:trashFolder -ItemType Directory
   $script:trashFolder = Resolve-Path -Path $script:trashFolder
}
