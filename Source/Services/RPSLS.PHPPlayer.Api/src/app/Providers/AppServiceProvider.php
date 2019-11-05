<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     *
     * @return void
     */
    public function register()
    {
        $this->app->singleton(PickStrategyFactory::class, function ($app) {
            return new PickStrategyFactory();
        });

        $this->app->singleton(PredictorProxy::class, function ($app) {
            return new PredictorProxy();
        });
    }
}
