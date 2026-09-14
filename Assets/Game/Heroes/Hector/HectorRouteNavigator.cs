using System.Collections.Generic;
using UnityEngine;

public static class HectorRouteNavigator
{
    const float SharedPointTolerance = .15f;

    struct Projection
    {
        public int routeIndex;
        public float distanceAlong;
        public Vector3 point;
        public float distanceSq;
    }

    public static bool HasRoutes(Transform[][] routes)
    {
        if (routes == null || routes.Length == 0) return false;
        for (int i = 0; i < routes.Length; i++)
            if (routes[i] != null && routes[i].Length >= 2) return true;
        return false;
    }

    public static Vector3 GateStart(Transform[][] routes, float height)
    {
        if (!HasRoutes(routes)) return Vector3.zero;

        Transform[] primary = FirstValidRoute(routes);
        int index = Mathf.Max(0, primary.Length - 2);
        Vector3 point = primary[index].position;
        point.y = height;
        return point;
    }

    public static Vector3 ProjectToNearestRoute(Vector3 worldPosition, Transform[][] routes, float height)
    {
        if (!TryProject(worldPosition, routes, out Projection projection))
        {
            worldPosition.y = height;
            return worldPosition;
        }

        projection.point.y = height;
        return projection.point;
    }

    public static bool BuildPath(
        Vector3 from,
        Vector3 requestedDestination,
        Transform[][] routes,
        float height,
        List<Vector3> output)
    {
        output.Clear();
        if (!TryProject(from, routes, out Projection start) ||
            !TryProject(requestedDestination, routes, out Projection end))
            return false;

        start.point.y = height;
        end.point.y = height;

        if ((from - start.point).sqrMagnitude > .01f)
            AddDistinct(output, start.point);

        if (start.routeIndex == end.routeIndex)
        {
            AppendTravel(routes[start.routeIndex], start.distanceAlong, end.distanceAlong, height, end.point, output);
            return output.Count > 0;
        }

        if (!TryFindSharedJunction(routes[start.routeIndex], routes[end.routeIndex], height, out Vector3 junction))
        {
            AddDistinct(output, end.point);
            return output.Count > 0;
        }

        Projection startJunction = ProjectOnRoute(junction, routes[start.routeIndex], start.routeIndex);
        Projection endJunction = ProjectOnRoute(junction, routes[end.routeIndex], end.routeIndex);
        AppendTravel(routes[start.routeIndex], start.distanceAlong, startJunction.distanceAlong, height, junction, output);
        AppendTravel(routes[end.routeIndex], endJunction.distanceAlong, end.distanceAlong, height, end.point, output);
        return output.Count > 0;
    }

    static bool TryProject(Vector3 worldPosition, Transform[][] routes, out Projection best)
    {
        best = new Projection { routeIndex = -1, distanceSq = float.PositiveInfinity };
        if (!HasRoutes(routes)) return false;

        for (int routeIndex = 0; routeIndex < routes.Length; routeIndex++)
        {
            Transform[] route = routes[routeIndex];
            if (route == null || route.Length < 2) continue;
            Projection candidate = ProjectOnRoute(worldPosition, route, routeIndex);
            if (candidate.distanceSq >= best.distanceSq) continue;
            best = candidate;
        }

        return best.routeIndex >= 0;
    }

    static Projection ProjectOnRoute(Vector3 worldPosition, Transform[] route, int routeIndex)
    {
        Projection best = new Projection
        {
            routeIndex = routeIndex,
            distanceSq = float.PositiveInfinity,
            point = route != null && route.Length > 0 ? route[0].position : worldPosition
        };

        if (route == null || route.Length < 2) return best;

        float cumulative = 0f;
        Vector3 flatWorld = new Vector3(worldPosition.x, 0f, worldPosition.z);
        for (int i = 0; i < route.Length - 1; i++)
        {
            Vector3 a = route[i].position;
            Vector3 b = route[i + 1].position;
            a.y = 0f;
            b.y = 0f;
            Vector3 segment = b - a;
            float length = segment.magnitude;
            if (length <= .001f) continue;

            float t = Mathf.Clamp01(Vector3.Dot(flatWorld - a, segment) / (length * length));
            Vector3 point = a + segment * t;
            float distanceSq = (flatWorld - point).sqrMagnitude;
            if (distanceSq < best.distanceSq)
            {
                best.distanceSq = distanceSq;
                best.point = point;
                best.distanceAlong = cumulative + length * t;
            }
            cumulative += length;
        }

        return best;
    }

    static void AppendTravel(
        Transform[] route,
        float startDistance,
        float endDistance,
        float height,
        Vector3 finalPoint,
        List<Vector3> output)
    {
        if (route == null || route.Length < 2)
        {
            finalPoint.y = height;
            AddDistinct(output, finalPoint);
            return;
        }

        float[] distances = BuildVertexDistances(route);
        bool forward = endDistance >= startDistance;
        if (forward)
        {
            for (int i = 1; i < route.Length; i++)
            {
                float d = distances[i];
                if (d <= startDistance + .01f || d >= endDistance - .01f) continue;
                Vector3 point = route[i].position;
                point.y = height;
                AddDistinct(output, point);
            }
        }
        else
        {
            for (int i = route.Length - 2; i >= 0; i--)
            {
                float d = distances[i];
                if (d >= startDistance - .01f || d <= endDistance + .01f) continue;
                Vector3 point = route[i].position;
                point.y = height;
                AddDistinct(output, point);
            }
        }

        finalPoint.y = height;
        AddDistinct(output, finalPoint);
    }

    static float[] BuildVertexDistances(Transform[] route)
    {
        float[] distances = new float[route.Length];
        for (int i = 1; i < route.Length; i++)
        {
            Vector3 a = route[i - 1].position;
            Vector3 b = route[i].position;
            a.y = 0f;
            b.y = 0f;
            distances[i] = distances[i - 1] + Vector3.Distance(a, b);
        }
        return distances;
    }

    static bool TryFindSharedJunction(Transform[] a, Transform[] b, float height, out Vector3 junction)
    {
        junction = Vector3.zero;
        if (a == null || b == null) return false;

        float toleranceSq = SharedPointTolerance * SharedPointTolerance;
        for (int i = 0; i < a.Length; i++)
        {
            Vector3 ap = a[i].position;
            ap.y = 0f;
            for (int j = 0; j < b.Length; j++)
            {
                Vector3 bp = b[j].position;
                bp.y = 0f;
                if ((ap - bp).sqrMagnitude > toleranceSq) continue;
                junction = (ap + bp) * .5f;
                junction.y = height;
                return true;
            }
        }
        return false;
    }

    static Transform[] FirstValidRoute(Transform[][] routes)
    {
        for (int i = 0; i < routes.Length; i++)
            if (routes[i] != null && routes[i].Length >= 2) return routes[i];
        return null;
    }

    static void AddDistinct(List<Vector3> output, Vector3 point)
    {
        if (output.Count > 0 && (output[output.Count - 1] - point).sqrMagnitude <= .0025f) return;
        output.Add(point);
    }
}
