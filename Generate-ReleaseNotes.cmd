rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.5.52

GitHubReleaseNotes --output CHANGELOG.md --skip-empty-releases --exclude-labels question invalid doc duplicate example --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels question invalid doc duplicate --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%