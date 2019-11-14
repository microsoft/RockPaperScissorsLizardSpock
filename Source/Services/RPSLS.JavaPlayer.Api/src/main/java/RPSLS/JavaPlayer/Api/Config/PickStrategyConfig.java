package RPSLS.JavaPlayer.Api.Config;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import RPSLS.JavaPlayer.Api.Strategy.PickStrategyFactory;
import RPSLS.JavaPlayer.Api.Service.PredictorProxyService;

@Configuration
public class PickStrategyConfig {
    Logger logger = LoggerFactory.getLogger(PickStrategyConfig.class);

    @Value("${rpsls.player.pick.strategy}")
    private String pickStrategyChoice;

    @Value("${rpsls.predictor.pick.url}")
    private String predictorPickUrl;

    @Bean
    public PredictorProxyService PredictorProxyService(){
        return new PredictorProxyService(predictorPickUrl);
    }
    
    @Bean
    public PickStrategyFactory PickStrategyFactory() {
        logger.info("Configured pick strategy with '{}'", pickStrategyChoice);
        PickStrategyFactory factory = new PickStrategyFactory();
        factory.setDefaultStrategy(pickStrategyChoice);
        return factory;
    }
}
