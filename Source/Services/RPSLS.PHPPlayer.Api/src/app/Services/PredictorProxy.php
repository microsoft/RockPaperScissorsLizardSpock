<?php

namespace App\Services;

use App\Services\RPSLSOptions;
use GuzzleHttp\Client;
use Exception;

class PredictorProxy
{
    public function getPickPredicted($username) 
    {
        $uriQueried = config('predictor.PREDICTOR_URL').'&humanPlayerName='.$username;
        $client = new Client();
        $response = $client->request('GET', $uriQueried);
        if ($response->getStatusCode() != 200) {
            throw new Exception('Predictor returned an error');
        }

        $result = json_decode($response->getBody());
        return RPSLSOptions::getpPickByText(strtolower($result->prediction));
    }
}