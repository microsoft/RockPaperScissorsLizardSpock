<?php

return [
    'PICK_STRATEGY' => getenv('PICK_STRATEGY') ?: env('PICK_STRATEGY', 'lizard'),
];