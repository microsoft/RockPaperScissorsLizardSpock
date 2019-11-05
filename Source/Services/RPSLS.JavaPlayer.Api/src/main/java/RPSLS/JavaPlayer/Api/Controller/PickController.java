package RPSLS.JavaPlayer.Api.Controller;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import RPSLS.JavaPlayer.Api.RPSLSDto;
import RPSLS.JavaPlayer.Api.Service.PredictorProxyService;
import RPSLS.JavaPlayer.Api.RPSLS;
import RPSLS.JavaPlayer.Api.Strategy.IPickStrategy;

@RestController
public class PickController {
    Logger logger = LoggerFactory.getLogger(PickController.class);

    @Autowired
    private IPickStrategy strategy;

    @Autowired
    private PredictorProxyService predictor;

    @Value("${rpsls.player.pick.strategy}")
    private String pickStrategyChoice;

    @RequestMapping("/pick")
    public RPSLSDto index(@RequestParam(value = "username", required = false) String username) {
        RPSLS pick;
        RPSLSDto result;
        if (!isStringNullOrEmpty(username)) {
            try {
                pick = predictor.getPickPredicted(username);
                result = new RPSLSDto(pick);
                logger.info("Against user [" + username + "] predictor played " + result.getText());
                return result;
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        pick = strategy.getPick();
        result = new RPSLSDto(pick);
        logger.info("Against some user, strategy " + pickStrategyChoice + "  played " + result.getText());
        return result;
    }
    
    private Boolean isStringNullOrEmpty(String str) {
        return str == null || str.trim().isEmpty();
    }
}