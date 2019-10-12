Task Publish -Depends Pack {
   Exec { docker login docker.io  --username=tiksn }
   $remoteTag = "docker.io/$script:latestImageTag"
   Exec { docker tag $script:latestImageTag $remoteTag }
   Exec { docker push $remoteTag }
}

Task Pack -Depends Build {
   $src = (Resolve-Path ".\src\").Path
   Exec { docker build -f Dockerfile $src -t $script:latestImageTag }
}

Task Build -Depends TranspileModels {
   $script:publishFolder = Join-Path -Path $script:trashFolder -ChildPath "publish"

   New-Item -Path $script:publishFolder -ItemType Directory
   $project = Resolve-Path ".\src\IdentityManagementService.csproj"
   $project = $project.Path
   Exec { dotnet publish $project --output $script:publishFolder }
}

Task TranspileModels -Depends Init,Clean {
   $apiModelYaml = (Resolve-Path ".\src\ApiModels.yml").Path
   $apiModelOutput = Join-Path -Path ".\src\Models" -ChildPath "ApiModels"
   Exec { smite --input-file $apiModelYaml --lang csharp --field property --output-folder $apiModelOutput }
}

Task Clean -Depends Init {
}

Task Init {
   $date = Get-Date
   $ticks = $date.Ticks
   $script:latestImageTag = "tiksn/lionize-identity-management-service:latest"
   $trashFolder = Join-Path -Path . -ChildPath ".trash"
   $script:trashFolder = Join-Path -Path $trashFolder -ChildPath $ticks.ToString("D19")
   New-Item -Path $script:trashFolder -ItemType Directory
   $script:trashFolder = Resolve-Path -Path $script:trashFolder
}
