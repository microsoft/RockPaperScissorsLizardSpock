<?php

namespace App\Http\Controllers;

use Laravel\Lumen\Routing\Controller as BaseController;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Request;
use App\Services\PickStrategyFactory;
use App\Services\PredictorProxy;
use App\Services\AppInsightsService;

class PickController extends BaseController
{
    private $_context;
    private $_strategy;
    private $_predictor;
    private $_appinsights;

    public function __construct(PickStrategyFactory $strategyFactory, PredictorProxy $predictorProxy, AppInsightsService $appinsights)
    {
        $this->_strategy = config('strategies.PICK_STRATEGY');
        $strategyFactory->setDefaultStrategy($this->_strategy);
        Log::info('Configured pick strategy with '.$this->_strategy);
        $this->_context = $strategyFactory->getStrategy();
        $this->_predictor = $predictorProxy;
        $this->_appinsights = $appinsights;
    }

    public function index() 
    {
        $username = Request::get('username', '');
        $this->_appinsights->trackRequest('PickController:index()', 'GET /pick', time());
        if($username != '') {
            try {
                $predicted = $this->_predictor->getPickPredicted($username);
                Log::info('Against user ['.$username.'] predictor played '.$predicted->text);
                return response()->json($predicted);
            } catch(\Throwable $e) {
                Log::error('Predictor had a problem');
                $this->_appinsights->trackException($e);
            }
        }

        $response = $this->_context->getChoice();
        Log::info('Against some user, strategy'.$this->_strategy.' played '.$response->text);
        return response()->json($response);
    }
}
