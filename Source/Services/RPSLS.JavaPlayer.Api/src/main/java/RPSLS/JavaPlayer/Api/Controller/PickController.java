package RPSLS.JavaPlayer.Api.Controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import RPSLS.JavaPlayer.Api.RPSLSDto;
import RPSLS.JavaPlayer.Api.Service.PredictorProxyService;
import RPSLS.JavaPlayer.Api.RPSLS;
import RPSLS.JavaPlayer.Api.Strategy.IPickStrategy;

@RestController
public class PickController {
    @Autowired
    private IPickStrategy strategy;

    @Autowired
    private PredictorProxyService predictor;

    @RequestMapping("/pick")
    public RPSLSDto index(@RequestParam(value = "username", required = false) String username) {
        RPSLS pick;
        if (!isStringNullOrEmpty(username)) {
            try {
                pick = predictor.getPickPredicted(username);
                return new RPSLSDto(pick);
            } catch (Exception e) {
                e.printStackTrace();
            }
        }

        pick = strategy.getPick();
        return new RPSLSDto(pick);
    }
    
    private Boolean isStringNullOrEmpty(String str) {
        return str == null || str.trim().isEmpty();
    }
}