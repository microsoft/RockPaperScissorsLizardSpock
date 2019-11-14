<?php

namespace App\Http\Controllers;

use Laravel\Lumen\Routing\Controller as BaseController;

class HealthController extends BaseController
{
    public function index() 
    {
        return response()->json(["status"=> "UP"]);
    }
}
