<?php

namespace App\Services;

class PickStrategyFactory 
{
    private const _ROCK = "rock";
    private const _PAPER = "paper";
    private const _SCISSORS = "scissors";
    private const _LIZARD = "lizard";
    private const _SPOCK = "spock";
    private const _RANDOM = "random";
    private const _ITERATIVE = "iterative";

    private $_defaultStrategy = "random";

    public function getStrategy() 
    {
        switch($this->_defaultStrategy) 
        {
            case self::_ROCK:
                return new PickContext(new FixedStrategy(0));
            case self::_PAPER:
                return new PickContext(new FixedStrategy(1));
            case self::_SCISSORS:
                return new PickContext(new FixedStrategy(2));
            case self::_LIZARD:
                return new PickContext(new FixedStrategy(3));
            case self::_SPOCK:
                return new PickContext(new FixedStrategy(4));
            case self::_RANDOM:
                return new PickContext(new RandomStrategy());
            case self::_ITERATIVE:
                return new PickContext(new IterativeStrategy());
            default: 
                return new PickContext(new RandomStrategy());
        }
    }

    public function setDefaultStrategy($newDefaultStrategy)
    {
        if($newDefaultStrategy !== NULL)
        {
            $this->_defaultStrategy = strtolower($newDefaultStrategy);
        }
    }
}