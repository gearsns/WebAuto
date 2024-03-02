getWebAutoData = () => {
	retData = []
	mainContentCond = ".actionBody"
	getActionBodyElement = () => {
		if (document.location.href.match(/view$/) && !document.location.href.match(/\/(edit|new)\?/)) {
			return document.querySelector(".windowViewMode-normal.oneContent.active")
		}
		return document.querySelector(mainContentCond)
	}
	matchAttrName = (node, name, cond) => (node && node[name] && node[name].match && node[name].match(cond))
	findNode = (node, func) => {
		const treeWalker = document.createTreeWalker(
			node,
			NodeFilter.SHOW_ELEMENT
		)
		while(treeWalker.nextNode())
		{
			const ret = func(treeWalker.currentNode)
			if(ret){
				return ret
			}
		}
		return null
	}
	findNodeMatchAttrName = (node, name, cond) => findNode(node, c_node => matchAttrName(c_node, name, cond) ? c_node : null)
	findNode(getActionBodyElement(), record_node => {
		if (matchAttrName(record_node, "tagName", /records\-record\-layout\-item/i)) {
			const label_name = record_node.getAttribute("field-label")
			if(!label_name){
				return null
			}
			let value = null
			if (null === value) {
				const el_value = findNodeMatchAttrName(record_node, "tagName", /lightning\-formatted\-rich\-text/i)
				if (el_value) {
					value = el_value.innerText
				}
			}
			if (null === value) {
				const el_datepicker = findNodeMatchAttrName(record_node, "tagName", /lightning\-datepicker/i)
				if (el_datepicker) {
					const el_input = findNodeMatchAttrName(el_datepicker, "tagName", /^(?:input|textarea)/i)
					if (el_input) {
						value = el_input.value
					}
				}
				const el_timepicker = findNodeMatchAttrName(record_node, "tagName", /lightning\-timepicker/i)
				if (el_timepicker) {
					const el_input = findNodeMatchAttrName(el_timepicker, "tagName", /^(?:input|textarea)/i)
					if (el_input) {
						if (null === value) {
							value = el_input.value
						} else {
							value += ` ${el_input.value}`
						}
					}
				}
			}
			if (null === value) {
				const el_value = findNodeMatchAttrName(record_node, "tagName", /lightning\-formatted\-(?:text|number)/i)
				if (el_value) {
					value = el_value.innerText
				}
			}
			if (null === value) {
				const el_link = findNodeMatchAttrName(record_node, "tagName", /records\-hoverable\-link/i)
				if (el_link) {
					const el_anchor = findNodeMatchAttrName(el_link, "tagName", /^a/i)
					if (el_anchor) {
						value = el_anchor.innerText
					}
				}
			}
			if (null === value) {
				const el_value = findNodeMatchAttrName(record_node, "tagName", /lightning\-base\-combobox/i)
				if (el_value) {
					const el_input = findNodeMatchAttrName(el_value, "tagName", /^(?:input|textarea)/i)
					if (el_input) {
						value = el_input.value
					} else {
						value = el_value.innerText
					}
				}
			}
			if (null === value) {
				const el_input = findNodeMatchAttrName(record_node, "tagName", /^(?:input|textarea)/i)
				if (el_input) {
					if ("checkbox" === el_input.type) {
						value = el_input.checked ? "on" : ""
					} else {
						value = el_input.value
					}
				}
			}
			if (null === value) {
				const el_span = findNodeMatchAttrName(record_node, "className", /^displayLabel/i)
				if (el_span) {
					value = el_span.innerText
				}
			}
			retData.push({
				"text": label_name
				, "name": label_name.replace(/^\*/, "")
				, "value": value
				, "note": ""
				, "type": ""
			})
			return null
		}
		return null
	})

	return {
		header: [
			{ name: "名前", key: "text" }
			, { name: "キー", key: "name" }
			, { name: "値", key: "value" }
			, { name: "備考", key: "note" }
			, { name: "タイプ", key: "type" }
		]
		, data: retData
	}
}