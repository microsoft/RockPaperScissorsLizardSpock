const PickStrategyFactory = require('../services/pickStrategyFactory.service');
const PredictorProxy = require('../services/predictorProxy.service');

const pick = async (req, res) => {    
    if(req.query.username) {
        try {
            const predictor = new PredictorProxy();
            const result = await predictor.getPickPredicted(req.query.username);
            res.send(result);    
        } catch (error) {
            console.error(error);
            res.send(pickFromDefaultStrategy());    
        }
    }
    else {        
        res.send(pickFromDefaultStrategy());
    }
};

const pickFromDefaultStrategy = () => {
    const strategyOption = process.env.PICK_STRATEGY;
    const strategyFactory = new PickStrategyFactory();

    strategyFactory.setDefaultStrategy(strategyOption);
    const strategy = strategyFactory.getStrategy();
    return strategy.pick();
}

module.exports = {
    pick,
}