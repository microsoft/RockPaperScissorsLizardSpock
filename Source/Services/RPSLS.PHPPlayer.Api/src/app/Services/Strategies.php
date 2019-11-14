<?php

namespace App\Services;

use Cache;

class RPSLSDto
{
    public $value;
    public $text;
    public $player;
    public $playerType;

    public function __construct($value, $text) 
    {
        $this->value = $value;
        $this->text = $text;
        $this->player = gethostname();
        $this->playerType="php";
    }
}

class RPSLSOptions 
{
    private const _ROCK = 0;
    private const _PAPER = 1;
    private const _SCISSORS = 2;
    private const _LIZARD = 3;
    private const _SPOCK = 4;

    public static function getpPickByValue($value)
    {
        return new RPSLSDto($value, self::CHOICES[$value]);
    }

    public static function getpPickByText($text)
    {
        return new RPSLSDto(array_search($text, self::CHOICES), $text);
    }

    const CHOICES = [
        self::_ROCK => "rock",
        self::_PAPER => "paper",
        self::_SCISSORS => "scissors",
        self::_LIZARD => "lizard",
        self::_SPOCK => "spock",
    ];
}

interface Strategy
{
    public function doPickAlgorithm();
}

class RandomStrategy implements Strategy
{
    public function doPickAlgorithm()
    {
        $randomChoice = rand(0, count(RPSLSOptions::CHOICES)-1);
        return RPSLSOptions::getpPickByValue($randomChoice);
    }
}

class IterativeStrategy implements Strategy
{
    public function doPickAlgorithm()
    {
        $current = Cache::get('ITERATIVE_COUNTER');

        if($current >= (count(RPSLSOptions::CHOICES)-1))
        {
            $current = -1;
        }

        $current++;
        Cache::put('ITERATIVE_COUNTER', $current);
        return RPSLSOptions::getpPickByValue($current);
    }
}

class FixedStrategy implements Strategy 
{
    private $_choiceDto;

    public function __construct($choice)
    {
        $this->_choiceDto = RPSLSOptions::getpPickByValue($choice);
    }

    public function doPickAlgorithm()
    {
        return $this->_choiceDto;
    }
}