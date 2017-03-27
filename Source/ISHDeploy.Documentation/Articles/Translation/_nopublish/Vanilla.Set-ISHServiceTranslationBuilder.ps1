# Set the Translation Builder configuration
# All parameters are optional

$hash=@{
	MaxObjectsInOnePush=1000
	MaxJobItemsCreatedInOneCall=10000
	CompletedJobLifeSpan="90.00:00:00"
	JobProcessingTimeout="01:00:00"
	JobPollingInterval="00:05:00"
	PendingJobPollingInterval="00:15:00"
}

Set-ISHServiceTranslationBuilder -ISHDeployment $deploymentName @hash