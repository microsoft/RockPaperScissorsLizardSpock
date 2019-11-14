const health = (req, res) => res.json({status: 'UP'});

module.exports = {
    health,
}