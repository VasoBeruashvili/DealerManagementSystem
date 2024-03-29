﻿/*
 Highcharts JS v4.1.10 (2015-12-07)

 (c) 2009-2014 Torstein Honsi

 License: www.highcharts.com/license
*/
(function (m) { typeof module === "object" && module.exports ? module.exports = m : m(Highcharts) })(function (m) {
    function K(a, b, c) { this.init(a, b, c) } var P = m.arrayMin, Q = m.arrayMax, s = m.each, H = m.extend, t = m.merge, R = m.map, p = m.pick, A = m.pInt, o = m.getOptions().plotOptions, i = m.seriesTypes, u = m.extendClass, L = m.splat, r = m.wrap, M = m.Axis, z = m.Tick, I = m.Point, S = m.Pointer, T = m.CenteredSeriesMixin, B = m.TrackerMixin, w = m.Series, x = Math, E = x.round, C = x.floor, N = x.max, U = m.Color, v = function () { }; H(K.prototype, {
        init: function (a, b, c) {
            var d = this, e =
                d.defaultOptions; d.chart = b; d.options = a = t(e, b.angular ? { background: {} } : void 0, a); (a = a.background) && s([].concat(L(a)).reverse(), function (a) { var b = a.backgroundColor, j = c.userOptions, a = t(d.defaultBackgroundOptions, a); if (b) a.backgroundColor = b; a.color = a.backgroundColor; c.options.plotBands.unshift(a); j.plotBands = j.plotBands || []; j.plotBands !== c.options.plotBands && j.plotBands.unshift(a) })
        }, defaultOptions: { center: ["50%", "50%"], size: "85%", startAngle: 0 }, defaultBackgroundOptions: {
            shape: "circle", borderWidth: 1, borderColor: "silver",
            backgroundColor: { linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 }, stops: [[0, "#FFF"], [1, "#DDD"]] }, from: -Number.MAX_VALUE, innerRadius: 0, to: Number.MAX_VALUE, outerRadius: "105%"
        }
    }); var G = M.prototype, z = z.prototype, V = { getOffset: v, redraw: function () { this.isDirty = !1 }, render: function () { this.isDirty = !1 }, setScale: v, setCategories: v, setTitle: v }, O = {
        isRadial: !0, defaultRadialGaugeOptions: {
            labels: { align: "center", x: 0, y: null }, minorGridLineWidth: 0, minorTickInterval: "auto", minorTickLength: 10, minorTickPosition: "inside", minorTickWidth: 1,
            tickLength: 10, tickPosition: "inside", tickWidth: 2, title: { rotation: 0 }, zIndex: 2
        }, defaultRadialXOptions: { gridLineWidth: 1, labels: { align: null, distance: 15, x: 0, y: null }, maxPadding: 0, minPadding: 0, showLastLabel: !1, tickLength: 0 }, defaultRadialYOptions: { gridLineInterpolation: "circle", labels: { align: "right", x: -3, y: -2 }, showLastLabel: !1, title: { x: 4, text: null, rotation: 90 } }, setOptions: function (a) { a = this.options = t(this.defaultOptions, this.defaultRadialOptions, a); if (!a.plotBands) a.plotBands = [] }, getOffset: function () {
            G.getOffset.call(this);
            this.chart.axisOffset[this.side] = 0; this.center = this.pane.center = T.getCenter.call(this.pane)
        }, getLinePath: function (a, b) { var c = this.center, b = p(b, c[2] / 2 - this.offset); return this.chart.renderer.symbols.arc(this.left + c[0], this.top + c[1], b, b, { start: this.startAngleRad, end: this.endAngleRad, open: !0, innerR: 0 }) }, setAxisTranslation: function () {
            G.setAxisTranslation.call(this); if (this.center) this.transA = this.isCircular ? (this.endAngleRad - this.startAngleRad) / (this.max - this.min || 1) : this.center[2] / 2 / (this.max - this.min ||
                1), this.minPixelPadding = this.isXAxis ? this.transA * this.minPointOffset : 0
        }, beforeSetTickPositions: function () { this.autoConnect && (this.max += this.categories && 1 || this.pointRange || this.closestPointRange || 0) }, setAxisSize: function () { G.setAxisSize.call(this); if (this.isRadial) { this.center = this.pane.center = m.CenteredSeriesMixin.getCenter.call(this.pane); if (this.isCircular) this.sector = this.endAngleRad - this.startAngleRad; this.len = this.width = this.height = this.center[2] * p(this.sector, 1) / 2 } }, getPosition: function (a,
            b) { return this.postTranslate(this.isCircular ? this.translate(a) : 0, p(this.isCircular ? b : this.translate(a), this.center[2] / 2) - this.offset) }, postTranslate: function (a, b) { var c = this.chart, d = this.center, a = this.startAngleRad + a; return { x: c.plotLeft + d[0] + Math.cos(a) * b, y: c.plotTop + d[1] + Math.sin(a) * b } }, getPlotBandPath: function (a, b, c) {
                var d = this.center, e = this.startAngleRad, f = d[2] / 2, h = [p(c.outerRadius, "100%"), c.innerRadius, p(c.thickness, 10)], j = /%$/, n, g = this.isCircular; this.options.gridLineInterpolation === "polygon" ?
                    d = this.getPlotLinePath(a).concat(this.getPlotLinePath(b, !0)) : (a = Math.max(a, this.min), b = Math.min(b, this.max), g || (h[0] = this.translate(a), h[1] = this.translate(b)), h = R(h, function (a) { j.test(a) && (a = A(a, 10) * f / 100); return a }), c.shape === "circle" || !g ? (a = -Math.PI / 2, b = Math.PI * 1.5, n = !0) : (a = e + this.translate(a), b = e + this.translate(b)), d = this.chart.renderer.symbols.arc(this.left + d[0], this.top + d[1], h[0], h[0], { start: Math.min(a, b), end: Math.max(a, b), innerR: p(h[1], h[0] - h[2]), open: n })); return d
            }, getPlotLinePath: function (a,
                b) { var c = this, d = c.center, e = c.chart, f = c.getPosition(a), h, j, n; c.isCircular ? n = ["M", d[0] + e.plotLeft, d[1] + e.plotTop, "L", f.x, f.y] : c.options.gridLineInterpolation === "circle" ? (a = c.translate(a)) && (n = c.getLinePath(0, a)) : (s(e.xAxis, function (a) { a.pane === c.pane && (h = a) }), n = [], a = c.translate(a), d = h.tickPositions, h.autoConnect && (d = d.concat([d[0]])), b && (d = [].concat(d).reverse()), s(d, function (f, b) { j = h.getPosition(f, a); n.push(b ? "L" : "M", j.x, j.y) })); return n }, getTitlePosition: function () {
                    var a = this.center, b = this.chart,
                    c = this.options.title; return { x: b.plotLeft + a[0] + (c.x || 0), y: b.plotTop + a[1] - { high: 0.5, middle: 0.25, low: 0 }[c.align] * a[2] + (c.y || 0) }
                }
    }; r(G, "init", function (a, b, c) {
        var k; var d = b.angular, e = b.polar, f = c.isX, h = d && f, j, n; n = b.options; var g = c.pane || 0; if (d) { if (H(this, h ? V : O), j = !f) this.defaultRadialOptions = this.defaultRadialGaugeOptions } else if (e) H(this, O), this.defaultRadialOptions = (j = f) ? this.defaultRadialXOptions : t(this.defaultYAxisOptions, this.defaultRadialYOptions); a.call(this, b, c); if (!h && (d || e)) {
            a = this.options; if (!b.panes) b.panes =
                []; this.pane = (k = b.panes[g] = b.panes[g] || new K(L(n.pane)[g], b, this), g = k); g = g.options; b.inverted = !1; n.chart.zoomType = null; this.startAngleRad = b = (g.startAngle - 90) * Math.PI / 180; this.endAngleRad = n = (p(g.endAngle, g.startAngle + 360) - 90) * Math.PI / 180; this.offset = a.offset || 0; if ((this.isCircular = j) && c.max === void 0 && n - b === 2 * Math.PI) this.autoConnect = !0
        }
    }); r(z, "getPosition", function (a, b, c, d, e) { var f = this.axis; return f.getPosition ? f.getPosition(c) : a.call(this, b, c, d, e) }); r(z, "getLabelPosition", function (a, b, c, d, e, f, h,
        j, n) {
        var g = this.axis, l = f.y, k = 20, i = f.align, y = (g.translate(this.pos) + g.startAngleRad + Math.PI / 2) / Math.PI * 180 % 360; g.isRadial ? (a = g.getPosition(this.pos, g.center[2] / 2 + p(f.distance, -25)), f.rotation === "auto" ? d.attr({ rotation: y }) : l === null && (l = g.chart.renderer.fontMetrics(d.styles.fontSize).b - d.getBBox().height / 2), i === null && (g.isCircular ? (this.label.getBBox().width > g.len * g.tickInterval / (g.max - g.min) && (k = 0), i = y > k && y < 180 - k ? "left" : y > 180 + k && y < 360 - k ? "right" : "center") : i = "center", d.attr({ align: i })), a.x += f.x, a.y +=
            l) : a = a.call(this, b, c, d, e, f, h, j, n); return a
    }); r(z, "getMarkPath", function (a, b, c, d, e, f, h) { var j = this.axis; j.isRadial ? (a = j.getPosition(this.pos, j.center[2] / 2 + d), b = ["M", b, c, "L", a.x, a.y]) : b = a.call(this, b, c, d, e, f, h); return b }); o.arearange = t(o.area, {
        lineWidth: 1, marker: null, threshold: null, tooltip: { pointFormat: '<span style="color:{series.color}">\u25cf</span> {series.name}: <b>{point.low}</b> - <b>{point.high}</b><br/>' }, trackByArea: !0, dataLabels: { align: null, verticalAlign: null, xLow: 0, xHigh: 0, yLow: 0, yHigh: 0 },
        states: { hover: { halo: !1 } }
    }); i.arearange = u(i.area, {
        type: "arearange", pointArrayMap: ["low", "high"], dataLabelCollections: ["dataLabel", "dataLabelUpper"], toYData: function (a) { return [a.low, a.high] }, pointValKey: "low", deferTranslatePolar: !0, highToXY: function (a) { var b = this.chart, c = this.xAxis.postTranslate(a.rectPlotX, this.yAxis.len - a.plotHigh); a.plotHighX = c.x - b.plotLeft; a.plotHigh = c.y - b.plotTop }, getSegments: function () {
            var a = this; s(a.points, function (b) {
                if (!a.options.connectNulls && (b.low === null || b.high === null)) b.y =
                    null; else if (b.low === null && b.high !== null) b.y = b.high
            }); w.prototype.getSegments.call(this)
        }, translate: function () { var a = this, b = a.yAxis; i.area.prototype.translate.apply(a); s(a.points, function (a) { var d = a.low, e = a.high, f = a.plotY; e === null && d === null ? a.y = null : d === null ? (a.plotLow = a.plotY = null, a.plotHigh = b.translate(e, 0, 1, 0, 1)) : e === null ? (a.plotLow = f, a.plotHigh = null) : (a.plotLow = f, a.plotHigh = b.translate(e, 0, 1, 0, 1)) }); this.chart.polar && s(this.points, function (b) { a.highToXY(b) }) }, getSegmentPath: function (a) {
            var b,
            c = [], d = a.length, e = w.prototype.getSegmentPath, f, h; h = this.options; var j = h.step; for (b = m.grep(a, function (a) { return a.plotLow !== null }) ; d--;) f = a[d], f.plotHigh !== null && c.push({ plotX: f.plotHighX || f.plotX, plotY: f.plotHigh }); a = e.call(this, b); if (j) j === !0 && (j = "left"), h.step = { left: "right", center: "center", right: "left" }[j]; c = e.call(this, c); h.step = j; h = [].concat(a, c); this.chart.polar || (c[0] = "L"); this.areaPath = this.areaPath.concat(a, c); return h
        }, drawDataLabels: function () {
            var a = this.data, b = a.length, c, d = [], e = w.prototype,
            f = this.options.dataLabels, h = f.align, j = f.verticalAlign, n = f.inside, g, l, k = this.chart.inverted; if (f.enabled || this._hasPointLabels) {
                for (c = b; c--;) if (g = a[c]) { l = n ? g.plotHigh < g.plotLow : g.plotHigh > g.plotLow; g.y = g.high; g._plotY = g.plotY; g.plotY = g.plotHigh; d[c] = g.dataLabel; g.dataLabel = g.dataLabelUpper; g.below = l; if (k) { if (!h) f.align = l ? "right" : "left" } else if (!j) f.verticalAlign = l ? "top" : "bottom"; f.x = f.xHigh; f.y = f.yHigh } e.drawDataLabels && e.drawDataLabels.apply(this, arguments); for (c = b; c--;) if (g = a[c]) {
                    l = n ? g.plotHigh <
                        g.plotLow : g.plotHigh > g.plotLow; g.dataLabelUpper = g.dataLabel; g.dataLabel = d[c]; g.y = g.low; g.plotY = g._plotY; g.below = !l; if (k) { if (!h) f.align = l ? "left" : "right" } else if (!j) f.verticalAlign = l ? "bottom" : "top"; f.x = f.xLow; f.y = f.yLow
                } e.drawDataLabels && e.drawDataLabels.apply(this, arguments)
            } f.align = h; f.verticalAlign = j
        }, alignDataLabel: function () { i.column.prototype.alignDataLabel.apply(this, arguments) }, setStackedPoints: v, getSymbol: v, drawPoints: v
    }); o.areasplinerange = t(o.arearange); i.areasplinerange = u(i.arearange, {
        type: "areasplinerange",
        getPointSpline: i.spline.prototype.getPointSpline
    }); (function () {
        var a = i.column.prototype; o.columnrange = t(o.column, o.arearange, { lineWidth: 1, pointRange: null }); i.columnrange = u(i.arearange, {
            type: "columnrange", translate: function () { var b = this, c = b.yAxis, d; a.translate.apply(b); s(b.points, function (a) { var f = a.shapeArgs, h = b.options.minPointLength, j; a.tooltipPos = null; a.plotHigh = d = c.translate(a.high, 0, 1, 0, 1); a.plotLow = a.plotY; j = d; a = a.plotY - d; Math.abs(a) < h ? (h -= a, a += h, j -= h / 2) : a < 0 && (a *= -1, j -= a); f.height = a; f.y = j }) },
            directTouch: !0, trackerGroups: ["group", "dataLabelsGroup"], drawGraph: v, crispCol: a.crispCol, pointAttrToOptions: a.pointAttrToOptions, drawPoints: a.drawPoints, drawTracker: a.drawTracker, animate: a.animate, getColumnMetrics: a.getColumnMetrics
        })
    })(); o.gauge = t(o.line, { dataLabels: { enabled: !0, defer: !1, y: 15, borderWidth: 1, borderColor: "silver", borderRadius: 3, crop: !1, verticalAlign: "top", zIndex: 2 }, dial: {}, pivot: {}, tooltip: { headerFormat: "" }, showInLegend: !1 }); B = {
        type: "gauge", pointClass: u(I, {
            setState: function (a) {
                this.state =
                    a
            }
        }), angular: !0, drawGraph: v, fixedBox: !0, forceDL: !0, trackerGroups: ["group", "dataLabelsGroup"], translate: function () {
            var a = this.yAxis, b = this.options, c = a.center; this.generatePoints(); s(this.points, function (d) {
                var e = t(b.dial, d.dial), f = A(p(e.radius, 80)) * c[2] / 200, h = A(p(e.baseLength, 70)) * f / 100, j = A(p(e.rearLength, 10)) * f / 100, n = e.baseWidth || 3, g = e.topWidth || 1, l = b.overshoot, k = a.startAngleRad + a.translate(d.y, null, null, null, !0); l && typeof l === "number" ? (l = l / 180 * Math.PI, k = Math.max(a.startAngleRad - l, Math.min(a.endAngleRad +
                    l, k))) : b.wrap === !1 && (k = Math.max(a.startAngleRad, Math.min(a.endAngleRad, k))); k = k * 180 / Math.PI; d.shapeType = "path"; d.shapeArgs = { d: e.path || ["M", -j, -n / 2, "L", h, -n / 2, f, -g / 2, f, g / 2, h, n / 2, -j, n / 2, "z"], translateX: c[0], translateY: c[1], rotation: k }; d.plotX = c[0]; d.plotY = c[1]
            })
        }, drawPoints: function () {
            var a = this, b = a.yAxis.center, c = a.pivot, d = a.options, e = d.pivot, f = a.chart.renderer; s(a.points, function (b) {
                var c = b.graphic, e = b.shapeArgs, g = e.d, l = t(d.dial, b.dial); c ? (c.animate(e), e.d = g) : b.graphic = f[b.shapeType](e).attr({
                    stroke: l.borderColor ||
                    "none", "stroke-width": l.borderWidth || 0, fill: l.backgroundColor || "black", rotation: e.rotation, zIndex: 1
                }).add(a.group)
            }); c ? c.animate({ translateX: b[0], translateY: b[1] }) : a.pivot = f.circle(0, 0, p(e.radius, 5)).attr({ "stroke-width": e.borderWidth || 0, stroke: e.borderColor || "silver", fill: e.backgroundColor || "black", zIndex: 2 }).translate(b[0], b[1]).add(a.group)
        }, animate: function (a) {
            var b = this; if (!a) s(b.points, function (a) {
                var d = a.graphic; d && (d.attr({ rotation: b.yAxis.startAngleRad * 180 / Math.PI }), d.animate({ rotation: a.shapeArgs.rotation },
                    b.options.animation))
            }), b.animate = null
        }, render: function () { this.group = this.plotGroup("group", "series", this.visible ? "visible" : "hidden", this.options.zIndex, this.chart.seriesGroup); w.prototype.render.call(this); this.group.clip(this.chart.clipRect) }, setData: function (a, b) { w.prototype.setData.call(this, a, !1); this.processData(); this.generatePoints(); p(b, !0) && this.chart.redraw() }, drawTracker: B && B.drawTrackerPoint
    }; i.gauge = u(i.line, B); o.boxplot = t(o.column, {
        fillColor: "#FFFFFF", lineWidth: 1, medianWidth: 2, states: { hover: { brightness: -0.3 } },
        threshold: null, tooltip: { pointFormat: '<span style="color:{point.color}">\u25cf</span> <b> {series.name}</b><br/>Maximum: {point.high}<br/>Upper quartile: {point.q3}<br/>Median: {point.median}<br/>Lower quartile: {point.q1}<br/>Minimum: {point.low}<br/>' }, whiskerLength: "50%", whiskerWidth: 2
    }); i.boxplot = u(i.column, {
        type: "boxplot", pointArrayMap: ["low", "q1", "median", "q3", "high"], toYData: function (a) { return [a.low, a.q1, a.median, a.q3, a.high] }, pointValKey: "high", pointAttrToOptions: {
            fill: "fillColor", stroke: "color",
            "stroke-width": "lineWidth"
        }, drawDataLabels: v, translate: function () { var a = this.yAxis, b = this.pointArrayMap; i.column.prototype.translate.apply(this); s(this.points, function (c) { s(b, function (b) { c[b] !== null && (c[b + "Plot"] = a.translate(c[b], 0, 1, 0, 1)) }) }) }, drawPoints: function () {
            var a = this, b = a.options, c = a.chart.renderer, d, e, f, h, j, n, g, l, k, i, y, m, J, o, t, r, v, u, w, x, B, A, z = a.doQuartiles !== !1, F, D = a.options.whiskerLength; s(a.points, function (q) {
                k = q.graphic; B = q.shapeArgs; y = {}; o = {}; r = {}; A = q.color || a.color; if (q.plotY !== void 0) if (d =
                    q.pointAttr[q.selected ? "selected" : ""], v = B.width, u = C(B.x), w = u + v, x = E(v / 2), e = C(z ? q.q1Plot : q.lowPlot), f = C(z ? q.q3Plot : q.lowPlot), h = C(q.highPlot), j = C(q.lowPlot), y.stroke = q.stemColor || b.stemColor || A, y["stroke-width"] = p(q.stemWidth, b.stemWidth, b.lineWidth), y.dashstyle = q.stemDashStyle || b.stemDashStyle, o.stroke = q.whiskerColor || b.whiskerColor || A, o["stroke-width"] = p(q.whiskerWidth, b.whiskerWidth, b.lineWidth), r.stroke = q.medianColor || b.medianColor || A, r["stroke-width"] = p(q.medianWidth, b.medianWidth, b.lineWidth),
                    g = y["stroke-width"] % 2 / 2, l = u + x + g, i = ["M", l, f, "L", l, h, "M", l, e, "L", l, j], z && (g = d["stroke-width"] % 2 / 2, l = C(l) + g, e = C(e) + g, f = C(f) + g, u += g, w += g, m = ["M", u, f, "L", u, e, "L", w, e, "L", w, f, "L", u, f, "z"]), D && (g = o["stroke-width"] % 2 / 2, h += g, j += g, F = /%$/.test(D) ? x * parseFloat(D) / 100 : D / 2, J = ["M", l - F, h, "L", l + F, h, "M", l - F, j, "L", l + F, j]), g = r["stroke-width"] % 2 / 2, n = E(q.medianPlot) + g, t = ["M", u, n, "L", w, n], k) q.stem.animate({ d: i }), D && q.whiskers.animate({ d: J }), z && q.box.animate({ d: m }), q.medianShape.animate({ d: t }); else {
                    q.graphic = k = c.g().add(a.group);
                    q.stem = c.path(i).attr(y).add(k); if (D) q.whiskers = c.path(J).attr(o).add(k); if (z) q.box = c.path(m).attr(d).add(k); q.medianShape = c.path(t).attr(r).add(k)
                }
            })
        }, setStackedPoints: v
    }); o.errorbar = t(o.boxplot, { color: "#000000", grouping: !1, linkedTo: ":previous", tooltip: { pointFormat: '<span style="color:{point.color}">\u25cf</span> {series.name}: <b>{point.low}</b> - <b>{point.high}</b><br/>' }, whiskerWidth: null }); i.errorbar = u(i.boxplot, {
        type: "errorbar", pointArrayMap: ["low", "high"], toYData: function (a) {
            return [a.low,
                a.high]
        }, pointValKey: "high", doQuartiles: !1, drawDataLabels: i.arearange ? i.arearange.prototype.drawDataLabels : v, getColumnMetrics: function () { return this.linkedParent && this.linkedParent.columnMetrics || i.column.prototype.getColumnMetrics.call(this) }
    }); o.waterfall = t(o.column, { lineWidth: 1, lineColor: "#333", dashStyle: "dot", borderColor: "#333", dataLabels: { inside: !0 }, states: { hover: { lineWidthPlus: 0 } } }); i.waterfall = u(i.column, {
        type: "waterfall", upColorProp: "fill", pointValKey: "y", translate: function () {
            var a = this.options,
            b = this.yAxis, c, d, e, f, h, j, n, g, l, k = a.threshold, m = a.stacking; i.column.prototype.translate.apply(this); n = g = k; d = this.points; for (c = 0, a = d.length; c < a; c++) {
                e = d[c]; j = this.processedYData[c]; f = e.shapeArgs; l = (h = m && b.stacks[(this.negStacks && j < k ? "-" : "") + this.stackKey]) ? h[e.x].points[this.index + "," + c] : [0, j]; if (e.isSum) e.y = j; else if (e.isIntermediateSum) e.y = j - g; h = N(n, n + e.y) + l[0]; f.y = b.translate(h, 0, 1); if (e.isSum) f.y = b.translate(l[1], 0, 1), f.height = Math.min(b.translate(l[0], 0, 1), b.len) - f.y; else if (e.isIntermediateSum) f.y =
                    b.translate(l[1], 0, 1), f.height = Math.min(b.translate(g, 0, 1), b.len) - f.y, g = l[1]; else { if (n !== 0) f.height = j > 0 ? b.translate(n, 0, 1) - f.y : b.translate(n, 0, 1) - b.translate(n - j, 0, 1); n += j } f.height < 0 && (f.y += f.height, f.height *= -1); e.plotY = f.y = E(f.y) - this.borderWidth % 2 / 2; f.height = N(E(f.height), 0.001); e.yBottom = f.y + f.height; f = e.plotY + (e.negative ? f.height : 0); this.chart.inverted ? e.tooltipPos[0] = b.len - f : e.tooltipPos[1] = f
            }
        }, processData: function (a) {
            var b = this.yData, c = this.options.data, d, e = b.length, f, h, j, n, g, l; h = f = j = n = this.options.threshold ||
                0; for (l = 0; l < e; l++) g = b[l], d = c && c[l] ? c[l] : {}, g === "sum" || d.isSum ? b[l] = h : g === "intermediateSum" || d.isIntermediateSum ? b[l] = f : (h += g, f += g), j = Math.min(h, j), n = Math.max(h, n); w.prototype.processData.call(this, a); this.dataMin = j; this.dataMax = n
        }, toYData: function (a) { return a.isSum ? a.x === 0 ? null : "sum" : a.isIntermediateSum ? a.x === 0 ? null : "intermediateSum" : a.y }, getAttribs: function () {
            i.column.prototype.getAttribs.apply(this, arguments); var a = this, b = a.options, c = b.states, d = b.upColor || a.color, b = m.Color(d).brighten(0.1).get(),
                e = t(a.pointAttr), f = a.upColorProp; e[""][f] = d; e.hover[f] = c.hover.upColor || b; e.select[f] = c.select.upColor || d; s(a.points, function (f) { if (!f.options.color) f.y > 0 ? (f.pointAttr = e, f.color = d) : f.pointAttr = a.pointAttr })
        }, getGraphPath: function () { var a = this.data, b = a.length, c = E(this.options.lineWidth + this.borderWidth) % 2 / 2, d = [], e, f, h; for (h = 1; h < b; h++) f = a[h].shapeArgs, e = a[h - 1].shapeArgs, f = ["M", e.x + e.width, e.y + c, "L", f.x, e.y + c], a[h - 1].y < 0 && (f[2] += e.height, f[5] += e.height), d = d.concat(f); return d }, getExtremes: v, drawGraph: w.prototype.drawGraph
    });
    o.polygon = t(o.scatter, { marker: { enabled: !1 } }); i.polygon = u(i.scatter, { type: "polygon", fillGraph: !0, getSegmentPath: function (a) { return w.prototype.getSegmentPath.call(this, a).concat("z") }, drawGraph: w.prototype.drawGraph, drawLegendSymbol: m.LegendSymbolMixin.drawRectangle }); o.bubble = t(o.scatter, {
        dataLabels: { formatter: function () { return this.point.z }, inside: !0, verticalAlign: "middle" }, marker: { lineColor: null, lineWidth: 1 }, minSize: 8, maxSize: "20%", softThreshold: !1, states: { hover: { halo: { size: 5 } } }, tooltip: { pointFormat: "({point.x}, {point.y}), Size: {point.z}" },
        turboThreshold: 0, zThreshold: 0, zoneAxis: "z"
    }); B = u(I, { haloPath: function () { return I.prototype.haloPath.call(this, this.shapeArgs.r + this.series.options.states.hover.halo.size) }, ttBelow: !1 }); i.bubble = u(i.scatter, {
        type: "bubble", pointClass: B, pointArrayMap: ["y", "z"], parallelArrays: ["x", "y", "z"], trackerGroups: ["group", "dataLabelsGroup"], bubblePadding: !0, zoneAxis: "z", pointAttrToOptions: { stroke: "lineColor", "stroke-width": "lineWidth", fill: "fillColor" }, applyOpacity: function (a) {
            var b = this.options.marker, c = p(b.fillOpacity,
                0.5), a = a || b.fillColor || this.color; c !== 1 && (a = U(a).setOpacity(c).get("rgba")); return a
        }, convertAttribs: function () { var a = w.prototype.convertAttribs.apply(this, arguments); a.fill = this.applyOpacity(a.fill); return a }, getRadii: function (a, b, c, d) {
            var e, f, h, j = this.zData, n = [], g = this.options, l = g.sizeBy !== "width", k = g.zThreshold, i = b - a; for (f = 0, e = j.length; f < e; f++) h = j[f], g.sizeByAbsoluteValue && h !== null && (h = Math.abs(h - k), b = Math.max(b - k, Math.abs(a - k)), a = 0), h === null ? h = null : h < a ? h = c / 2 - 1 : (h = i > 0 ? (h - a) / i : 0.5, l && h >= 0 && (h = Math.sqrt(h)),
                h = x.ceil(c + h * (d - c)) / 2), n.push(h); this.radii = n
        }, animate: function (a) { var b = this.options.animation; if (!a) s(this.points, function (a) { var d = a.graphic, a = a.shapeArgs; d && a && (d.attr("r", 1), d.animate({ r: a.r }, b)) }), this.animate = null }, translate: function () {
            var a, b = this.data, c, d, e = this.radii; i.scatter.prototype.translate.call(this); for (a = b.length; a--;) c = b[a], d = e ? e[a] : 0, typeof d === "number" && d >= this.minPxSize / 2 ? (c.shapeType = "circle", c.shapeArgs = { x: c.plotX, y: c.plotY, r: d }, c.dlBox = {
                x: c.plotX - d, y: c.plotY - d, width: 2 * d,
                height: 2 * d
            }) : c.shapeArgs = c.plotY = c.dlBox = void 0
        }, drawLegendSymbol: function (a, b) { var c = this.chart.renderer, d = c.fontMetrics(a.itemStyle.fontSize).f / 2; b.legendSymbol = c.circle(d, a.baseline - d, d).attr({ zIndex: 3 }).add(b.legendGroup); b.legendSymbol.isMarker = !0 }, drawPoints: i.column.prototype.drawPoints, alignDataLabel: i.column.prototype.alignDataLabel, buildKDTree: v, applyZones: v
    }); M.prototype.beforePadding = function () {
        var a = this, b = this.len, c = this.chart, d = 0, e = b, f = this.isXAxis, h = f ? "xData" : "yData", j = this.min, n =
            {}, g = x.min(c.plotWidth, c.plotHeight), l = Number.MAX_VALUE, k = -Number.MAX_VALUE, i = this.max - j, m = b / i, o = []; s(this.series, function (b) {
                var h = b.options; if (b.bubblePadding && (b.visible || !c.options.chart.ignoreHiddenSeries)) if (a.allowZoomOutside = !0, o.push(b), f) s(["minSize", "maxSize"], function (a) { var b = h[a], f = /%$/.test(b), b = A(b); n[a] = f ? g * b / 100 : b }), b.minPxSize = n.minSize, b.maxPxSize = n.maxSize, b = b.zData, b.length && (l = p(h.zMin, x.min(l, x.max(P(b), h.displayNegative === !1 ? h.zThreshold : -Number.MAX_VALUE))), k = p(h.zMax, x.max(k,
                    Q(b))))
            }); s(o, function (a) { var b = a[h], c = b.length, g; f && a.getRadii(l, k, a.minPxSize, a.maxPxSize); if (i > 0) for (; c--;) typeof b[c] === "number" && (g = a.radii[c], d = Math.min((b[c] - j) * m - g, d), e = Math.max((b[c] - j) * m + g, e)) }); o.length && i > 0 && !this.isLog && (e -= b, m *= (b + d - e) / b, s([["min", "userMin", d], ["max", "userMax", e]], function (b) { p(a.options[b[0]], a[b[1]]) === void 0 && (a[b[0]] += b[2] / m) }))
    }; (function () {
        function a(a, b, c) {
            a.call(this, b, c); if (this.chart.polar) this.closeSegment = function (a) {
                var b = this.xAxis.center; a.push("L", b[0],
                    b[1])
            }, this.closedStacks = !0
        } function b(a, b) { var c = this.chart, d = this.options.animation, g = this.group, e = this.markerGroup, k = this.xAxis.center, i = c.plotLeft, m = c.plotTop; if (c.polar) { if (c.renderer.isSVG) d === !0 && (d = {}), b ? (c = { translateX: k[0] + i, translateY: k[1] + m, scaleX: 0.001, scaleY: 0.001 }, g.attr(c), e && e.attr(c)) : (c = { translateX: i, translateY: m, scaleX: 1, scaleY: 1 }, g.animate(c, d), e && e.animate(c, d), this.animate = null) } else a.call(this, b) } var c = w.prototype, d = S.prototype, e; c.searchPointByAngle = function (a) {
            var b = this.chart,
            c = this.xAxis.pane.center; return this.searchKDTree({ clientX: 180 + Math.atan2(a.chartX - c[0] - b.plotLeft, a.chartY - c[1] - b.plotTop) * (-180 / Math.PI) })
        }; r(c, "buildKDTree", function (a) { if (this.chart.polar) this.kdByAngle ? this.searchPoint = this.searchPointByAngle : this.kdDimensions = 2; a.apply(this) }); c.toXY = function (a) {
            var b, c = this.chart, d = a.plotX; b = a.plotY; a.rectPlotX = d; a.rectPlotY = b; b = this.xAxis.postTranslate(a.plotX, this.yAxis.len - b); a.plotX = a.polarPlotX = b.x - c.plotLeft; a.plotY = a.polarPlotY = b.y - c.plotTop; this.kdByAngle ?
                (c = (d / Math.PI * 180 + this.xAxis.pane.options.startAngle) % 360, c < 0 && (c += 360), a.clientX = c) : a.clientX = a.plotX
        }; i.area && r(i.area.prototype, "init", a); i.areaspline && r(i.areaspline.prototype, "init", a); i.spline && r(i.spline.prototype, "getPointSpline", function (a, b, c, d) {
            var g, e, k, i, m, o, p; if (this.chart.polar) {
                g = c.plotX; e = c.plotY; a = b[d - 1]; k = b[d + 1]; this.connectEnds && (a || (a = b[b.length - 2]), k || (k = b[1])); if (a && k) i = a.plotX, m = a.plotY, b = k.plotX, o = k.plotY, i = (1.5 * g + i) / 2.5, m = (1.5 * e + m) / 2.5, k = (1.5 * g + b) / 2.5, p = (1.5 * e + o) / 2.5, b = Math.sqrt(Math.pow(i -
                    g, 2) + Math.pow(m - e, 2)), o = Math.sqrt(Math.pow(k - g, 2) + Math.pow(p - e, 2)), i = Math.atan2(m - e, i - g), m = Math.atan2(p - e, k - g), p = Math.PI / 2 + (i + m) / 2, Math.abs(i - p) > Math.PI / 2 && (p -= Math.PI), i = g + Math.cos(p) * b, m = e + Math.sin(p) * b, k = g + Math.cos(Math.PI + p) * o, p = e + Math.sin(Math.PI + p) * o, c.rightContX = k, c.rightContY = p; d ? (c = ["C", a.rightContX || a.plotX, a.rightContY || a.plotY, i || g, m || e, g, e], a.rightContX = a.rightContY = null) : c = ["M", g, e]
            } else c = a.call(this, b, c, d); return c
        }); r(c, "translate", function (a) {
            var b = this.chart; a.call(this); if (b.polar &&
                (this.kdByAngle = b.tooltip && b.tooltip.shared, !this.preventPostTranslate)) { a = this.points; for (b = a.length; b--;) this.toXY(a[b]) }
        }); r(c, "getSegmentPath", function (a, b) { var c = this.points; if (this.chart.polar && this.options.connectEnds !== !1 && b[b.length - 1] === c[c.length - 1] && c[0].y !== null) this.connectEnds = !0, b = [].concat(b, [c[0]]); return a.call(this, b) }); r(c, "animate", b); if (i.column) e = i.column.prototype, r(e, "animate", b), r(e, "translate", function (a) {
            var b = this.xAxis, c = this.yAxis.len, d = b.center, e = b.startAngleRad, i =
                this.chart.renderer, k, m; this.preventPostTranslate = !0; a.call(this); if (b.isRadial) { b = this.points; for (m = b.length; m--;) k = b[m], a = k.barX + e, k.shapeType = "path", k.shapeArgs = { d: i.symbols.arc(d[0], d[1], c - k.plotY, null, { start: a, end: a + k.pointWidth, innerR: c - p(k.yBottom, c) }) }, this.toXY(k), k.tooltipPos = [k.plotX, k.plotY], k.ttBelow = k.plotY > d[1] }
        }), r(e, "alignDataLabel", function (a, b, d, e, g, i) {
            if (this.chart.polar) {
                a = b.rectPlotX / Math.PI * 180; if (e.align === null) e.align = a > 20 && a < 160 ? "left" : a > 200 && a < 340 ? "right" : "center"; if (e.verticalAlign ===
                    null) e.verticalAlign = a < 45 || a > 315 ? "bottom" : a > 135 && a < 225 ? "top" : "middle"; c.alignDataLabel.call(this, b, d, e, g, i)
            } else a.call(this, b, d, e, g, i)
        }); r(d, "getCoordinates", function (a, b) { var c = this.chart, d = { xAxis: [], yAxis: [] }; c.polar ? s(c.axes, function (a) { var e = a.isXAxis, f = a.center, i = b.chartX - f[0] - c.plotLeft, f = b.chartY - f[1] - c.plotTop; d[e ? "xAxis" : "yAxis"].push({ axis: a, value: a.translate(e ? Math.PI - Math.atan2(i, f) : Math.sqrt(Math.pow(i, 2) + Math.pow(f, 2)), !0) }) }) : d = a.call(this, b); return d })
    })()
});