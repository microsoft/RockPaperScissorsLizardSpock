const os = require("os");
const { RPSLSChoices } = require('../settings/rpslsOptions');

class FixedStrategy {

    constructor(choice) {
        this.choice = choice;
    }

    pick() {
        return {
            "playerType": "node",
            "player": os.hostname(),
            "text": this.choice,
            "value": RPSLSChoices.indexOf(this.choice)
        };
    }
}

module.exports = FixedStrategy;