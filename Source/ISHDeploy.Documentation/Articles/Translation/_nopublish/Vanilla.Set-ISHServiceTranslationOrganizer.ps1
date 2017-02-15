# Set the Translation Organizer configuration
# All parameters are optional

$hash=@{
	AttemptsBeforeFailOnRetrieval=3
	dumpFolder=$dumpPath
	JobPollingInterval="00:05:00"
    MaxTranslationJobItemsUpdatedInOneCall=100
    PendingJobPollingInterval="00:15:00"
    RetriesOnTimeout=3
    SystemTaskInterval="00:10:00"
    UpdateLeasedByPerNumberOfItems=100
}

Set-ISHServiceTranslationOrganizer -ISHDeployment $deploymentName @hash