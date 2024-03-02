class CancellationToken {
	isCancellationRequested = false;

	constructor() {
	}

	cancel() {
		this.isCancellationRequested = true;
	}
	reset() {
		this.isCancellationRequested = false;
	}
}
class SalesforceLightningAutoinput {
	mainContentCond = ".actionBody"
	cancellationToken = null
	constructor(m, ct = null) {
		if (m) {
			this.mainContentCond = m
		}
		this.cancellationToken = ct
	}

	_sleep = ms => {
		return new Promise((resolve) => setTimeout(resolve, ms))
	}

	findNode = async (node, func) => {
		const treeWalker = document.createTreeWalker(
			node,
			NodeFilter.SHOW_ELEMENT
		)
		while(treeWalker.nextNode())
		{
			const ret = await func(treeWalker.currentNode)
			if(ret){
				return ret
			}
		}
		return null
	}

	getActionBodyElement = async () => {
		if (document.location.href.match(/view$/) && !document.location.href.match(/\/(edit|new)\?/)) {
			for (let i = 0; i < 10; i++) {
				if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
					return null
				}
				let e = document.querySelector(".windowViewMode-normal.oneContent.active")
				if (e) {
					return e
				}
				await this._sleep(100)
			}
		} else {
			for (let i = 0; i < 10; i++) {
				if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
					return null
				}
				let e = document.querySelector(this.mainContentCond)
				if (e) {
					return e
				}
				await this._sleep(100)
			}
		}
		//
		return null
	}

	matchAttrName = (node, name, cond) => (node && node[name] && node[name].match && node[name].match(cond))
	findNodeMatchAttrName = async (node, name, cond) => await this.findNode(node, c_node => this.matchAttrName(c_node, name, cond) ? c_node : null)

	getLabel = async (node, label_name) => {
		return await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "className", /(^| +)slds\-form\-element__label($| +)/)
				&& (c_node.innerText === `*${label_name}` || c_node.innerText === label_name)
			) {
				return c_node
			}
			return null
		})
	}

	listLabel = async el_actionbody => {
		let cur_section_name = ""
		return await this.findNode(el_actionbody, async node => {
			if (!node || !node.tagName || !node.tagName.match) {
				return null
			}
			if (this.matchAttrName(node, "className", /(^| +)section\-header\-title($| +)/)) {
				cur_section_name = node.innerText;
				console.log(`${cur_section_name}`)
				return null;
			} else if (this.matchAttrName(node, "className", /(^| +)slds\-form\-element__label($| +)/)) {
				console.log(`+ ${node.innerText}`)
			}
			return null
		})
	}
	setValueWithEvent = (node, value) => {
		node.value = value
		node.dispatchEvent(new Event("input"))
		node.dispatchEvent(new Event("change"))
		node.dispatchEvent(new Event("blur"))
	}
	fireClickEvent = (node) => {
		node.click()
		node.dispatchEvent(new Event("blur"))
		return node
	}

	inputTextArea = async (node, input_value) => {
		const el_input = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /textarea/i)) {
				return c_node
			}
			return null
		});
		if (el_input) {
			el_input.value = input_value
			return this.fireClickEvent(el_input)
		}
		return null
	}
	inputCombobox = async (node, input_value) => {
		const el_input = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /input|button/i)
				&& c_node.className.match(/slds\-input/) && c_node.className.match(/slds\-combobox__input/)) {
				return c_node
			}
			return null
		})
		if (this.matchAttrName(el_input, "className", /slds\-input/) && el_input.className.match(/slds\-combobox__input/)) {
			for (let i = 0; i < 50; i++) {
				if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
					return null
				}
				el_input.click()
				let el_item = await this.findNode(node, async c_node => {
					if (this.matchAttrName(c_node, "tagName", /lightning\-base\-combobox\-item/i)
						&& (c_node.getAttribute("data-value") === input_value
							|| c_node.innerText === input_value)
					) {
						return c_node
					}
					return null
				});
				if (el_item) {
					this.fireClickEvent(el_item)
					return node
				}
				await this._sleep(500)
			}
		}
		return null
	}
	inputGroupedCombobox = async (node, input_value) => {
		await this.findNode(node, async c_button => {
			if (this.matchAttrName(c_button, "tagName", /button/i)
				&& this.matchAttrName(c_button, "title", "選択をクリア")) {
				return this.fireClickEvent(c_button)
			}
			return null
		})
		const el_input = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /input|button/i)
				&& c_node.className.match(/slds\-input/) && c_node.className.match(/slds\-combobox__input/)) {
				return c_node
			}
			return null
		})
		if (this.matchAttrName(el_input, "className", /slds\-input/) && el_input.className.match(/slds\-combobox__input/)) {
			this.setValueWithEvent(el_input, input_value)
			let check_value = input_value.split(/ /).sort().join(" ").toUpperCase()
			node.dispatchEvent(new CustomEvent("textinput", { "detail": { "text": input_value } }))
			for (let i = 0; i < 50; i++) {
				if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
					return null
				}
				el_input.dispatchEvent(new KeyboardEvent("keydown", { key: "a" }))
				el_input.click()
				let cc_node = await this.findNode(node, async cc_node => {
					if (this.matchAttrName(cc_node, "className", /(^| +)slds\-listbox__option\-(text_entity|meta_entity)($| +)/)
						&& cc_node.innerText && cc_node.innerText.split(/ /).sort().join(" ").toUpperCase() === check_value) {
						return cc_node
					}
					return null
				})
				if (cc_node) {
					cc_node.focus()
					return this.fireClickEvent(cc_node)
				}
				// actionAdvancedSearch
				let el_item_search = await this.findNode(node, async c_node => {
					if (this.matchAttrName(c_node, "tagName", /lightning\-base\-combobox\-item/i)
						&& (c_node.getAttribute("data-value") === "actionAdvancedSearch"
							|| c_node.innerText === input_value)
					) {
						return c_node
					}
					return null
				})
				if (el_item_search) {
					el_item_search.click()
					for (let i2 = 0; i2 < 50; i2++) {
						if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
							return null
						}
						let el_modal = document.querySelector(".forceSearchLookupAdvanced");
						if (el_modal) {
							for (let t of el_modal.querySelectorAll(".forceOutputLookup")) {
								if (t.title === input_value) {
									t.focus()
									t.click()
									return t
								}
							}
						}
						await this._sleep(500)
					}
				}
				await this._sleep(500)
				{
					const waitNode = await this.findNode(node, async c_node => {
						if (c_node.innerText && c_node.innerText.match(/読み込み中|選択リストからオプションを/)) {
							return c_node
						}
						return null
					})
					if (waitNode) {
						console.log("読み込み中")
						await this._sleep(500)
					}
				}
			}
		}
		return null
	}
	inputInputAddress = async (node, input_value) => {
		await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /(lightning\-input|lightning\-textarea)/i)) {
				let attr = c_node.getAttribute("data-field")
				if (input_value[attr]) {
					setValueWithEvent(c_node, input_value[attr])
				}
				return null
			}
		});
		return node
	}
	inputInput = async (node, input_value) => {
		let elDateTimePicker = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /lightning\-datetimepicker/i)) {
				return c_node
			}
			return null
		});
		if (elDateTimePicker) {
			let value_date = input_value
			let value_time = "";
			if (input_value.date) {
				value_date = input_value.date
			}
			if (input_value.time) {
				value_time = input_value.time
			}
			if (!input_value.date && !input_value.time) {
				const m = input_value.match(/(.*) (.*)/)
				if (m.length >= 2) {
					value_date = m[1]
					value_time = m[2]
				}
			}
			await this.findNode(elDateTimePicker, async c_node => {
				if (this.matchAttrName(c_node, "tagName", /(lightning\-datepicker|lightning\-timepicker)/i)) {
					if (c_node.tagName.match(/datepicker/i)) {
						await this.findNode(c_node, async cc_node => {
							if (this.matchAttrName(cc_node, "tagName", /input/i)) {
								this.setValueWithEvent(cc_node, value_date)
							}
							return null
						})
					} else {
						await this.findNode(c_node, async cc_node => {
							if (this.matchAttrName(cc_node, "tagName", /input/i)) {
								this.setValueWithEvent(cc_node, value_time)
							}
							return null
						})
					}
				}
				return null
			})
			return node
		}
		let elDateOrTimePicker = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /(lightning\-datepicker|lightning\-timepicker)/i)) {
				return await this.findNode(c_node, async cc_node => {
					if (this.matchAttrName(cc_node, "tagName", /input/i)) {
						this.setValueWithEvent(cc_node, input_value)
						return cc_node
					}
					return null
				})
			}
			return null
		});
		if (elDateOrTimePicker) {
			return node
		}
		const el_input = await this.findNode(node, async c_node => {
			if (this.matchAttrName(c_node, "tagName", /input/i)) {
				return c_node
			}
			return null
		});
		if (el_input) {
			if (el_input.type === "checkbox") {
				if (input_value === "check") {
					el_input.checked = true
				} else {
					el_input.checked = false
				}
			} else {
				await this.findNode(el_input, async c_node => {
					if (this.matchAttrName(c_node, "tagName", /input/i)) {
						c_node.click()
						this.setValueWithEvent(c_node, input_value)
					}
				})
				el_input.value = input_value
				el_input.click()
				el_input.dispatchEvent(new Event("input"))
				el_input.dispatchEvent(new Event("blur"))
			}
			return node
		}
	}

	inputValue = async (el_actionbody, label_name, input_value, section_name) => {
		// ラベルで検索
		const record_node = await this.findNode(el_actionbody, record_node => {
			return (this.matchAttrName(record_node, "tagName", /records\-record\-layout\-item/i)
				&& record_node.getAttribute("field-label") === label_name)
				? record_node : null
		})
		if (null === record_node) {
			console.log(`対応する項目が見つかりませんでした。${label_name}`)
			return null
		}
		let ret = await this.findNode(record_node, async node => {
			if (this.matchAttrName(node, "tagName", /lightning\-textarea/i)) {
				return await this.inputTextArea(node, input_value)
			} else if (this.matchAttrName(node, "tagName", /lightning\-combobox/i)) {
				return await this.inputCombobox(node, input_value)
			} else if (this.matchAttrName(node, "tagName", /lightning\-grouped\-combobox/i)) { // 所有者とか
				return await this.inputGroupedCombobox(node, input_value)
			} else if (this.matchAttrName(node, "tagName", /lightning\-input\-address/i)) {
				return await this.inputInputAddress(node, input_value)
			} else if (this.matchAttrName(node, "tagName", /lightning\-input/i)) {
				return await this.inputInput(node, input_value)
			}
			return null
		})
		if (!ret) {
			ret = await this.findNode(el_actionbody, async node => {
				if (!node || !node.tagName || !node.tagName.match) {
					return null
				}
				if (node.tagName === "LABEL" && node.innerText === label_name) {
					let el_input = el_actionbody.querySelector(`[id="${node.getAttribute("for")}"]`)
					if (el_input) {
						if (el_input.type === "checkbox") {
							console.log(el_input.type)
							el_input.checked = (input_value === "check")
							return node
						}
					}
				} else if (node.tagName === "INPUT" && node.getAttribute("title") === label_name) {
					let el_input = node
					if (el_input.className.match(/(^| +)uiInputTextForAutocomplete($| +)/)) {
						this.setValueWithEvent(el_input, input_value)
						let check_value = input_value.split(/ /).sort().join(" ").toUpperCase()
						node.dispatchEvent(new CustomEvent("textinput", { "detail": { "text": input_value } }))
						for (let i = 0; i < 50; i++) {
							if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
								return null
							}
							el_input.dispatchEvent(new KeyboardEvent("keydown", { key: "a" }))
							el_input.click()
							let cc_node = await this.findNode(node.parentNode, async cc_node => {
								if (this.matchAttrName(cc_node, "className", /(^| +)slds\-lookup__result\-text($| +)/)
									&& cc_node.innerText && cc_node.innerText.split(/ /).sort().join(" ").toUpperCase() === check_value) {
									return cc_node
								}
								return null
							})
							if (cc_node) {
								cc_node.focus()
								return this.fireClickEvent(cc_node)
							}
							await this._sleep(500)
						}
					}
				}
			})
		}
		return ret
	}

	anchorClick = async (el_actionbody, name, wait) => {
		return await this.findNode(el_actionbody, async node => {
			if (!node || !node.tagName || !node.tagName.match) {
				return null;
			}
			if (node.tagName === "A" && node.innerText && node.innerText.trim() === name) {
				node.click();
				return node;
			}
			return null;
		})
	}
	buttonClick = async (el_actionbody, name, wait) => {
		return await this.findNode(el_actionbody, async node => {
			if (this.matchAttrName(node, "tagName", /lightning\-button/i) && node.innerText === name) {
				const formURL = document.location.href
				let bSaveAndNew = await this.findNode(node, async c_node => {
					if (c_node.name === "SaveAndNew") {
						return c_node;
					}
					return null;
				})
				//
				let attr = el_actionbody.getAttribute("data-aura-rendered-by");
				node.click();
				if (wait) {
					// プログレスが表示されている間は待機
					console.log("プログレスが表示されている間は待機")
					await this._sleep(1000);
					for (let i = 0; i < 50; ++i) {
						if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
							return null;
						}
						if (!await this.findNode(el_actionbody, async c_node => 
							this.matchAttrName(c_node, "tagName", /lightning\-spinner/i)
							? c_node : null
						)) {
							break;
						}
						await this._sleep(500);
					}
					//
					if (bSaveAndNew) {
						// Wait for the new modal to be created.
						console.log("新しいモーダル画面が表示されるまで待機")
						for (let i = 0; i < 50; ++i) {
							if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
								return null;
							}
							let el_actionbody_new = await this.getActionBodyElement();
							if (el_actionbody_new) {
								if (el_actionbody_new.getAttribute("data-aura-rendered-by") !== attr) {
									break;
								}
							}
							await this._sleep(500);
						}
						// Wait for the button to be enabled.
						console.log("保存 & 新規ボタンが有効になるまで待機")
						let el_actionbody_tmp = await this.getActionBodyElement();
						if (el_actionbody_tmp) {
							for (let i = 0; i < 50; ++i) {
								console.log(`${i + 1}回目`)
								if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
									return null;
								}
								if (await this.findNode(el_actionbody_tmp, c_node => 
									((c_node.name === "SaveAndNew") ? c_node : null)
								)) {
									break;
								}
								await this._sleep(500);
							}
						}
						console.log("待機終了")
					}
					if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
						return null;
					}
					await this._sleep(1000);
				}
				return node;
			}
			return null;
		})
	}
	buttonWait = async (el_actionbody, name) => {
		for (let i = 0; i < 50; ++i) {
			if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
				return null;
			}
			let ret = await this.findNode(el_actionbody, async node => {
				if (!node || !node.tagName || !node.tagName.match) {
					return null;
				}
				if (node.tagName.match(/lightning\-button/i) && node.innerText === name) {
					// Wait for the button to be enabled.
					let el_actionbody_tmp = await this.getActionBodyElement();
					if (el_actionbody_tmp) {
						if (await this.findNode(el_actionbody_tmp, async c_node => {
							if (c_node.innerText === name) {
								console.log(c_node.className);
								await this._sleep(500);
								return c_node;
							}
							return null;
						})) {
							return node;
						}
					}
				}
				return null;
			});
			if (ret) {
				return ret;
			}
			await this._sleep(500);
		}
		return null;
	}

	run = async (key, value, section) => {
		console.log("start")
		if (key.type === "anchor") {
			await this.anchorClick(await this.getActionBodyElement(), value, key.wait)
		} else if (key.type === "button") {
			await this.buttonClick(await this.getActionBodyElement(), value, key.wait)
		} else if (key.type === "wait") {
			await this.buttonWait(await this.getActionBodyElement(), value)
		} else {
			for (let i = 0; i < 50; ++i) {
				if (this.cancellationToken && this.cancellationToken.isCancellationRequested) {
					break
				}
				const el_actionbody = await this.getActionBodyElement()
				if (el_actionbody) {
					if (await this.inputValue(el_actionbody, key, value, section)) {
						break
					}
				}
				console.log(`Wait：${i + 1}:${key}=${value}`)
				await this._sleep(50)
			}
		}
		console.log("end")
	}
}

WebAutoCancel = () => {
	if (g_cancel) {
		g_cancel.cancel()
	}
}
WebAutoSetValue = async (key, value) => {
	g_running = true;
	console.log(`開始：${key}=${value}`)
	if (key === "form") {
		if (value.length === 0) {
			window["lmisauto-form"] = null
		} else {
			window["lmisauto-form"] = value
		}
	} else if (key === "anchor") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(window["lmisauto-form"], g_cancel);
		await slai.run({ type: "anchor", wait: false }, value);
	} else if (key === "click") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(window["lmisauto-form"], g_cancel);
		await slai.run({ type: "button", wait: false }, value);
	} else if (key === "wait") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(window["lmisauto-form"], g_cancel);
		await slai.run({ type: "wait", wait: false }, value);
	} else if (key === "input") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(window["lmisauto-form"], g_cancel);
		await slai.run("ユーザを検索", value);
	} else if (key === "input1") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(window["lmisauto-form"], g_cancel);
		await slai.run("メールで通知する", value);
	} else if (key === "新規") {
	} else if (key === "保存") {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(null, g_cancel);
		await slai.run({ type: "button", wait: true }, "保存 & 新規");
	} else {
		if (!g_cancel) {
			g_cancel = new CancellationToken();
		}
		const slai = new SalesforceLightningAutoinput(null, g_cancel);
		await slai.run(key, value);
	}
	console.log(`終了：${key}=${value}`);
	g_running = false;
	if (g_cancel) {
		g_cancel.reset();
	}
	chrome.webview.hostObjects.WebAuto.CompletedScript()
}

let g_running = false;
let g_cancel = null;
