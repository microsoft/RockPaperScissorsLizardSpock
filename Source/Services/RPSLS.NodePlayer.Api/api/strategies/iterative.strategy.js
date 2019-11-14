const os = require("os");
const { RPSLSChoices } = require('../settings/rpslsOptions');

class IterativeStrategy {
    counter() {

    }

    constructor() {
        if(!!IterativeStrategy.instance) {            
            return IterativeStrategy.instance;
        }

        this._counter = 0;
        IterativeStrategy.instance = this;
        return this;
    }

    pick() {
        if(this._counter > RPSLSChoices.length-1) {
            this._counter = 0;
        }

        return {
            "player":  os.hostname(),
            "playerType": "node",
            "text": RPSLSChoices[this._counter],
            "value":  this._counter++
        };
    }
}

module.exports = IterativeStrategy;