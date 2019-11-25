<?php

namespace App\Services;

class AppInsightsService
{
    private $_telemetryClient;

    public function __construct()
    {
        $key = config('appinsights.APPLICATION_INSIGHTS_IKEY');
        $this->_telemetryClient = new \ApplicationInsights\Telemetry_Client();
        $context = $this->_telemetryClient->getContext();
        $context->setInstrumentationKey($key);
    }

    public function trackRequest($name, $url, $startTime)
    {
        $this->_telemetryClient->trackRequest($name, $url, $startTime);
        $this->_telemetryClient->flush();
    }

    public function trackException($ex)
    {
        $telemetryClient->trackException($ex);
        $telemetryClient->flush();
    }
}