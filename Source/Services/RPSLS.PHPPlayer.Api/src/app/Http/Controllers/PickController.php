<?php

namespace App\Http\Controllers;

use Laravel\Lumen\Routing\Controller as BaseController;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Request;
use App\Services\PickStrategyFactory;
use App\Services\PredictorProxy;

class PickController extends BaseController
{
    private $_context;
    private $_strategy;
    private $_predictor;

    public function __construct(PickStrategyFactory $strategyFactory, PredictorProxy $predictorProxy)
    {
        $this->_strategy = config('strategies.PICK_STRATEGY');
        $strategyFactory->setDefaultStrategy($this->_strategy);
        Log::info('Configured pick strategy with '.$this->_strategy);
        $this->_context = $strategyFactory->getStrategy();
        $this->_predictor = $predictorProxy;
    }

    public function index() 
    {
        $username = Request::get('username', '');

        if($username != '') {
            try {
                $predicted = $this->_predictor->getPickPredicted($username);
                return response()->json($predicted);
            }
            catch(Exception $e) {
                Log::error($e->getMessage());
            }
        }

        $response = $this->_context->getChoice();
        return response()->json($response);
    }
}
