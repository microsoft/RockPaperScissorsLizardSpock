<?php

namespace App\Services;

class PickContext
{
    private $_strategy;

    public function __construct(Strategy $strategy) 
    {
        $this->_strategy = $strategy;
    }

    public function getChoice() 
    {
        return $this->_strategy->doPickAlgorithm();
    }
}