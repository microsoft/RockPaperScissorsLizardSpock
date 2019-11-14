<?php

namespace App\Services;

use App\Services\RPSLSOptions;

class PredictorProxy
{
    public function getPickPredicted($username) 
    {
        $uriQueried = config('predictor.PREDICTOR_URL').'&humanPlayerName='.$username;
        $result = json_decode(file_get_contents($uriQueried));
        return RPSLSOptions::getpPickByText(strtolower($result->prediction));
    }
}