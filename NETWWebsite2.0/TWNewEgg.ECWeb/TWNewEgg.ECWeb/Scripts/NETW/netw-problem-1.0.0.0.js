//檢查問題單類種類對應的主旨
function CheckProblemType(ProblemType, ProblemKeynote) {
	var result;
	switch (ProblemType) {
	    case "Salceorder": {
			if (ProblemKeynote == 101 ||
				ProblemKeynote == 108 ||
				ProblemKeynote == 112 ||
				ProblemKeynote == 103 ||
				ProblemKeynote == 124) {
				result = true;
			}
			else {
				result = false;
			}
			break;
		}
	    case "Retgood": {
			if (ProblemKeynote == 115 ||
				ProblemKeynote == 116 ||
				ProblemKeynote == 114 ||
				ProblemKeynote == 117) {
				result = true;
			}
			else {
				result = false;
			}
			break;
		}
	    case "Invoice": {
			if (ProblemKeynote == 102 ||
				ProblemKeynote == 118 ||
				ProblemKeynote == 119 ||
				ProblemKeynote == 120) {
				result = true;
			}
			else {
				result = false;
			}
			break;
		}
	    case "Item": {
			if (ProblemKeynote == 113 ||
				ProblemKeynote == 121 ||
				ProblemKeynote == 105) {
				result = true;
			}
			else {
				result = false;
			}
			break;
		}
	    case "Other": {
			if (ProblemKeynote == 123 ||
				ProblemKeynote == 122 ||
				ProblemKeynote == 106) {
				result = true;
			}
			else {
				result = false;
			}
			break;
	    }
	    default: {
	        result = false;
	    }
	}
	return result;
}