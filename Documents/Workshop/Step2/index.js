const express = require('express');
const app = express();
const PORT = process.env.PORT || 1337;
app.use(express.static('public'));
app.listen(PORT, () => console.log(`Listening on ${ PORT }`));