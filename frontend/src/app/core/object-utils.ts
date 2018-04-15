export class ObjectUtils {
	static deepCopy(oldObj): any {
		let newObj = oldObj;
		if (oldObj && typeof oldObj === 'object') {
			newObj = Object.prototype.toString.call(oldObj) === '[object Array]' ? [] : {};
			for (let i in oldObj) {
				if (Object.prototype.toString.call(oldObj[i]) === '[object Date]') {
					newObj[i] = new Date(oldObj[i]);
				} else {
					newObj[i] = this.deepCopy(oldObj[i]);
				}
			}
		}
		return newObj;
	}

	static deepEqualWithEvery(x, y) {
		return (x && y && typeof x === 'object' && typeof y === 'object') ?
			(Object.keys(x).length === Object.keys(y).length) &&
			Object.keys(x).every(function(key) {
				return ObjectUtils.deepEqualWithEvery(x[key], y[key]);
			}, true) : (x === y);
	}

	static fillObject(outerObj, innerObj): any {
		let newObj = outerObj;
		if (outerObj && typeof outerObj === 'object' && innerObj && typeof innerObj === 'object') {
			newObj = Object.prototype.toString.call(outerObj) === '[object Array]' ? [] : {};
			for (let i in innerObj) {
				if (Object.prototype.toString.call(innerObj[i]) === '[object Date]') {
					newObj[i] = new Date(innerObj[i]);
				} else {
					newObj[i] = this.fillObject(outerObj[i], innerObj[i]);
				}
			}
		}
		return newObj;
	}
}

export class ArrayUtils {
	static findByProperty(arr: any[], propertyName: string, propertyValue: any) {
		return arr.find((obj: any) => obj[propertyName] === propertyValue);
	}

	static sortByField(array: any[], field: string, sortOrder: number = 1) {
		return array.sort(function (a, b) {
			let x = a[field];
			let y = b[field];

			if (sortOrder < 0) {
				return ((x < y) ? 1 : ((x > y) ? -1 : 0));
			}
			return ((x < y) ? -1 : ((x > y) ? 1 : 0));
		});
	}
}
