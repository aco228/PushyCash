
#-----------------------------------------------------------------------------------------------------
#	LINK STARTING CAMPAIGN 
#	()

	# Main values for startup
	DontStartNewCampaigns= true
	MaximumNumberOfActiveCampaigns= 15
	MaximumNumberOfCampaigns= 150
	IgnoreCountries=

	# Values for Link startup of campaign
	MinutesTimeoutAfterLinkStartedCampaign_WithNoConversions			= 60
	MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndMinusROI	= 30
	MinutesTimeoutAfterLinkStartedCampaign_WithConversionsAndPlusROI	= 15
	
	# how many minutes active campaign will run when there are no conversions (and nothing else is stoping it)
	MaximumMinutesForCampaignWithNoConversions=30
	
	# TrafficSourceUpdateStatsWorker (how much seconds is timeout for getting stats from TN)
	SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign=20
	SecondsForTrafficSourceUpdateStats_WithActiveCampaign=5
  
  
#-----------------------------------------------------------------------------------------------------
#	TRAFFIC NETWORK CAMPAIGN OPTIMIZATION 
#	(values for TryOptimization() method inside PushyNetworkCampaign.cs)

	OptimizationMinutesTimeout=2

#-----------------------------------------------------------------------------------------------------
#	INTERESTING OFFERS
#	( values for tracking new interesting offers )

	InterestingOffersMinutes=1
	InterestingOffersConversions=10

#-----------------------------------------------------------------------------------------------------
#	PushyNetworkCampaign CLICK MANAGEMENT
#	( maximum values for starting new campaign )
#

	MaximumClicksForConversions=950
	MinimumClicksForConversions=300
	MaximumBudget = 2.5
	MaximumBudgetForTrafficCampaign=5
	
	# how much conversions will be before we track deficit from maximumBudget
	MaximumConversionsForDeficitTracking=3



