rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.4.34

GitHubReleaseNotes --output CHANGELOG.md --skip-empty-releases --exclude-labels question invalid doc duplicate --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels question invalid doc duplicate --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%