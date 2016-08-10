import jobs.generation.Utilities
import jobs.generation.InternalUtilities

def project = GithubProject
def branch = GithubBranchName

// Generate a PR job for debug (test only), which just does testing.
['Debug', 'Release'].each { config ->
    def lowerCaseConfig = config.toLowerCase()

    def newJobName = InternalUtilities.getFullJobName(project, "windows_$lowerCaseConfig", true /* isPR */)

    def newJob = job(newJobName) {
        steps {
            batchFile("build.cmd -Configuration $config")
        }
    }

    Utilities.setMachineAffinity(newJob, 'Windows_NT', 'latest-or-auto-internal')
    InternalUtilities.standardJobSetup(newJob, project, true /* isPR */, "*/${branch}")
    Utilities.addXUnitDotNETResults(newJob, "bin/$config/Tests/TestResults.xml", false)

    Utilities.addGithubPRTriggerForBranch(newJob, branch, "Windows $config")
}