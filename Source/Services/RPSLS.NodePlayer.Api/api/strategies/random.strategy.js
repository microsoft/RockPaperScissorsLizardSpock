const os = require("os");
const { RPSLSChoices } = require('../settings/rpslsOptions');

class RandomStrategy {

    pick() {
        const choiceIndex = Math.floor((Math.random() * RPSLSChoices.length - 1) + 1);

        return {
            "player":  os.hostname(),
            "playerType": "node",
            "text": RPSLSChoices[choiceIndex],
            "value": choiceIndex
        };
    }
}

module.exports = RandomStrategy;