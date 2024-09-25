rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.6.4

GitHubReleaseNotes --output CHANGELOG.md --skip-empty-releases --exclude-labels question invalid doc duplicate example environment --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels question invalid doc duplicate example environment --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%