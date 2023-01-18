const express = require("express");
const router = express.Router()
const DataController = require('../controllers/dataController')

router.post('/', DataController.create)
router.get('/', DataController.getAll)

module.exports = router